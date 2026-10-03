using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace EZInfiniteYTLive
{
    /// <summary>
    /// Describes an error raised by the background streaming worker.
    /// </summary>
    public sealed class VideoStreamerErrorEventArgs : EventArgs
    {
        public VideoStreamerErrorEventArgs(string message, string filePath, int? exitCode, Exception exception)
        {
            Message = message;
            FilePath = filePath;
            ExitCode = exitCode;
            Exception = exception;
        }

        public string Message { get; private set; }
        public string FilePath { get; private set; }
        public int? ExitCode { get; private set; }
        public Exception Exception { get; private set; }
    }

    public class VideoStreamer : IDisposable
    {
        private const int MaxFfmpegErrorOutputLength = 12000;
        private const int UserMessageLength = 2000;

        private readonly string _ffmpegPath;
        private readonly string _videoFolder;
        private readonly string _rtmpUrl;
        private readonly bool _shuffle;
        private readonly string[] _videoExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".flv" };
        private readonly ManualResetEventSlim _stopEvent = new ManualResetEventSlim(false);
        private readonly Random _random = new Random();

        private Thread _streamThread;
        private volatile bool _stopRequested;
        private volatile bool _disposed;
        private volatile Process _ffmpegProcess;

        /// <summary>
        /// Raised when the streamer cannot scan the folder, start FFmpeg, or FFmpeg
        /// exits unsuccessfully. The event is raised on the streaming thread.
        /// </summary>
        public event EventHandler<VideoStreamerErrorEventArgs> ErrorOccurred;

        // Keep the original constructor for callers that do not need shuffle mode.
        public VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl)
            : this(ffmpegPath, videoFolder, rtmpUrl, false)
        {
        }

        public VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl, bool shuffle)
        {
            _ffmpegPath = ffmpegPath;
            _videoFolder = videoFolder;
            _rtmpUrl = rtmpUrl;
            _shuffle = shuffle;
        }

        /// <summary>
        /// Resolves an FFmpeg executable name using the current directory and PATH.
        /// This lets the UI fail before a worker thread is started when FFmpeg is
        /// missing, while still allowing an explicit absolute or relative path.
        /// </summary>
        public static bool TryResolveFfmpegPath(string ffmpegPath, out string resolvedPath, out string errorMessage)
        {
            resolvedPath = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(ffmpegPath))
            {
                errorMessage = "FFmpeg path is empty. Install FFmpeg or configure a valid executable path.";
                return false;
            }

            string requestedPath = ffmpegPath.Trim().Trim('"');
            var candidates = new List<string>();
            bool containsDirectory = Path.IsPathRooted(requestedPath)
                || requestedPath.IndexOf(Path.DirectorySeparatorChar) >= 0
                || requestedPath.IndexOf(Path.AltDirectorySeparatorChar) >= 0;

            Action<string> addCandidate = candidate =>
            {
                if (string.IsNullOrWhiteSpace(candidate))
                    return;

                if (!candidates.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                    candidates.Add(candidate);

                if (string.IsNullOrEmpty(Path.GetExtension(candidate)))
                {
                    string exeCandidate = candidate + ".exe";
                    if (!candidates.Contains(exeCandidate, StringComparer.OrdinalIgnoreCase))
                        candidates.Add(exeCandidate);
                }
            };

            if (containsDirectory)
            {
                addCandidate(requestedPath);
            }
            else
            {
                // Process.Start also searches the current directory and the
                // application's directory before PATH.
                addCandidate(Path.Combine(Environment.CurrentDirectory, requestedPath));
                addCandidate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, requestedPath));

                string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                foreach (string directory in path.Split(Path.PathSeparator))
                {
                    string trimmedDirectory = directory.Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(trimmedDirectory))
                        addCandidate(Path.Combine(trimmedDirectory, requestedPath));
                }
            }

            foreach (string candidate in candidates)
            {
                try
                {
                    if (File.Exists(candidate))
                    {
                        resolvedPath = Path.GetFullPath(candidate);
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore malformed PATH entries and continue searching.
                }
                catch (IOException)
                {
                    // Ignore an inaccessible candidate and continue searching.
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore an inaccessible candidate and continue searching.
                }
            }

            errorMessage = string.Format(
                "FFmpeg was not found at '{0}' or on PATH. Install FFmpeg and make sure ffmpeg.exe is available.",
                requestedPath);
            return false;
        }

        public void StartStreaming()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_streamThread != null && _streamThread.IsAlive)
                return;

            _stopRequested = false;
            _stopEvent.Reset();
            _streamThread = new Thread(StreamVideos)
            {
                IsBackground = true,
                Name = "EZInfiniteYTLive video streamer"
            };
            _streamThread.Start();
        }

        public void StopStreaming()
        {
            _stopRequested = true;
            try
            {
                _stopEvent.Set();
            }
            catch (ObjectDisposedException)
            {
                // The worker is already being disposed.
            }

            Process process = _ffmpegProcess;
            try
            {
                if (process != null && !process.HasExited)
                    process.Kill();
            }
            catch
            {
                // The process may have exited between HasExited and Kill.
            }

            Thread thread = _streamThread;
            if (thread != null && thread != Thread.CurrentThread && thread.IsAlive)
            {
                // Killing FFmpeg normally makes this return immediately. The timeout
                // prevents a broken child process from blocking the UI forever.
                thread.Join(TimeSpan.FromSeconds(2));
            }
        }

        private void StreamVideos()
        {
            bool noFilesReported = false;
            string lastScanError = null;

            try
            {
                string resolvedPath;
                string pathError;
                if (!TryResolveFfmpegPath(_ffmpegPath, out resolvedPath, out pathError))
                {
                    ReportError(pathError, null, null, null);
                    return;
                }

                while (!_stopRequested)
                {
                    string scanError;
                    List<string> files = GetVideoFiles(out scanError);

                    if (!string.IsNullOrEmpty(scanError))
                    {
                        if (!string.Equals(scanError, lastScanError, StringComparison.Ordinal))
                        {
                            ReportError(scanError, null, null, null);
                            lastScanError = scanError;
                        }
                    }
                    else
                    {
                        lastScanError = null;
                    }

                    if (files.Count == 0)
                    {
                        if (!noFilesReported)
                        {
                            ReportError(
                                "No supported video files were found. Add a video to the selected folder or one of its subfolders.",
                                null,
                                null,
                                null);
                            noFilesReported = true;
                        }

                        // Re-scan while waiting so a video added after START is
                        // picked up without requiring a restart.
                        WaitForStop(1000);
                        continue;
                    }

                    noFilesReported = false;
                    ShuffleFilesIfRequested(files);

                    // This is a snapshot for one pass only. A new snapshot is taken
                    // after every pass, so additions and removals are observed while
                    // the stream is running.
                    bool anyFilePlayedSuccessfully = false;
                    foreach (string file in files)
                    {
                        if (_stopRequested)
                            break;

                        // A file may have been removed after the directory scan.
                        // Skip it instead of starting FFmpeg with a stale path.
                        if (!File.Exists(file))
                            continue;

                        if (RunFfmpeg(file, resolvedPath))
                            anyFilePlayedSuccessfully = true;
                    }

                    // Avoid a tight retry loop when every file fails immediately
                    // (for example, because of an unsupported codec or RTMP error).
                    if (!anyFilePlayedSuccessfully && !_stopRequested)
                        WaitForStop(1000);
                }
            }
            catch (Exception ex)
            {
                // No exception from the worker should bring down the WinForms
                // process. In particular, this handles unexpected Process.Start
                // and filesystem failures outside the normal paths above.
                if (!_stopRequested)
                    ReportError("The streaming worker stopped unexpectedly: " + ex.Message, null, null, ex);
            }
        }

        private List<string> GetVideoFiles(out string errorMessage)
        {
            errorMessage = null;

            try
            {
                return Directory.GetFiles(_videoFolder, "*", SearchOption.AllDirectories)
                    .Where(f => _videoExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                errorMessage = "Unable to scan the video folder: " + ex.Message;
                return new List<string>();
            }
        }

        private void ShuffleFilesIfRequested(List<string> files)
        {
            if (!_shuffle)
                return;

            // Fisher-Yates gives every file one position in each pass and avoids
            // the bias and repeated comparisons of OrderBy(Guid.NewGuid()).
            for (int i = files.Count - 1; i > 0; i--)
            {
                int swapIndex = _random.Next(i + 1);
                string current = files[i];
                files[i] = files[swapIndex];
                files[swapIndex] = current;
            }
        }

        private bool RunFfmpeg(string inputFile, string ffmpegPath)
        {
            // The outer loop is responsible for replaying a file. Running FFmpeg
            // once here prevents every file from being played twice per pass.
            var args = string.Format(
                "-re -i \"{0}\" -c copy -f flv \"{1}\"",
                inputFile,
                _rtmpUrl);
            var psi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var stderr = new StringBuilder();
            using (var process = new Process { StartInfo = psi })
            {
                _ffmpegProcess = process;
                int exitCode = -1;
                bool started = false;

                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    // Reading stdout asynchronously prevents a verbose FFmpeg
                    // process from blocking on a full redirected output buffer.
                };
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    if (eventArgs.Data == null)
                        return;

                    lock (stderr)
                    {
                        stderr.AppendLine(eventArgs.Data);
                        if (stderr.Length > MaxFfmpegErrorOutputLength)
                        {
                            stderr.Remove(0, stderr.Length - MaxFfmpegErrorOutputLength);
                        }
                    }
                };

                try
                {
                    started = process.Start();
                    if (!started)
                    {
                        ReportError("FFmpeg could not be started.", inputFile, null, null);
                        return false;
                    }

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Handle a stop that raced with Process.Start().
                    if (_stopRequested)
                    {
                        try
                        {
                            if (!process.HasExited)
                                process.Kill();
                        }
                        catch
                        {
                        }
                    }

                    process.WaitForExit();
                    // Wait a second time for the asynchronous output callbacks to
                    // finish before reading the collected stderr text.
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }
                catch (Win32Exception ex)
                {
                    ReportError("Unable to start FFmpeg: " + ex.Message, inputFile, null, ex);
                    return false;
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is IOException)
                {
                    ReportError("FFmpeg could not process the file: " + ex.Message, inputFile, null, ex);
                    return false;
                }
                finally
                {
                    if (ReferenceEquals(_ffmpegProcess, process))
                        _ffmpegProcess = null;
                }

                if (started && exitCode != 0 && !_stopRequested)
                {
                    string detail;
                    lock (stderr)
                    {
                        detail = stderr.ToString().Trim();
                    }

                    if (detail.Length > UserMessageLength)
                        detail = detail.Substring(detail.Length - UserMessageLength);

                    string message = string.Format(
                        "FFmpeg failed for '{0}' with exit code {1}.",
                        inputFile,
                        exitCode);
                    if (!string.IsNullOrEmpty(detail))
                        message += Environment.NewLine + detail;

                    ReportError(message, inputFile, exitCode, null);
                }

                return started && exitCode == 0;
            }
        }

        private void WaitForStop(int milliseconds)
        {
            try
            {
                _stopEvent.Wait(milliseconds);
            }
            catch (ObjectDisposedException)
            {
                // Disposal is itself a stop request.
            }
        }

        private void ReportError(string message, string filePath, int? exitCode, Exception exception)
        {
            Trace.WriteLine(message);

            EventHandler<VideoStreamerErrorEventArgs> handler = ErrorOccurred;
            if (handler == null)
                return;

            try
            {
                handler(this, new VideoStreamerErrorEventArgs(message, filePath, exitCode, exception));
            }
            catch
            {
                // A UI/logging subscriber must not be able to terminate the worker.
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopStreaming();
            _disposed = true;
            _stopEvent.Dispose();
        }
    }
}
