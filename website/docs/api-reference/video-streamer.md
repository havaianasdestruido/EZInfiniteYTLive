---
id: video-streamer
title: VideoStreamer
sidebar_position: 3
---

# `VideoStreamer` class

**File:** [`EZInfiniteYTLive/VideoStreamer.cs`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/VideoStreamer.cs)
**Namespace:** `EZInfiniteYTLive`
**Type:** `public class VideoStreamer : IDisposable`

`VideoStreamer` is the core engine of the application. It has no dependency on WinForms and could, in principle, be reused from a console app or service. It is responsible for:

1. Enumerating supported video files in a folder and its subfolders.
2. Running FFmpeg against each file, one at a time, targeting an RTMP URL.
3. Re-scanning the folder after every pass so files added or removed while streaming are reflected without a restart.
4. Looping back to the first file once the last one finishes, forever, until stopped.

## Fields

| Field | Type | Description |
|---|---|---|
| `_ffmpegPath` | `readonly string` | Path or command name used to launch FFmpeg (e.g. `"ffmpeg.exe"`). |
| `_videoFolder` | `readonly string` | Root folder to scan recursively for video files. |
| `_rtmpUrl` | `readonly string` | Full destination RTMP URL (server + stream key). |
| `_shuffle` | `readonly bool` | Whether each pass should be randomized. |
| `_videoExtensions` | `readonly string[]` | Supported file extensions: `.mp4`, `.mkv`, `.avi`, `.mov`, `.flv`. |
| `_streamThread` | `Thread` | The background thread running the streaming loop. |
| `_stopRequested` | `bool` | Cooperative cancellation flag checked by the streaming loop. |
| `_ffmpegProcess` | `Process` | Reference to the currently running FFmpeg process, so it can be killed on demand. |

## Constructor

### `VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl, bool shuffle)`

```csharp
public VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl, bool shuffle)
```

Stores the streaming configuration. The three-argument constructor is retained for callers that want alphabetic ordering and is equivalent to passing `false` for `shuffle`.

| Parameter | Description |
|---|---|
| `ffmpegPath` | Executable name/path for FFmpeg. It may be an absolute path, a relative path, or a command available on `PATH`. |
| `videoFolder` | Root directory containing the video files to stream. Subfolders are scanned too. |
| `rtmpUrl` | Fully-formed RTMP destination, e.g. `rtmp://a.rtmp.youtube.com/live2/<stream-key>`. |
| `shuffle` | When `true`, uses a Fisher-Yates shuffle for every complete pass; otherwise files are sorted by filename. |

### `TryResolveFfmpegPath`

```csharp
public static bool TryResolveFfmpegPath(
    string ffmpegPath,
    out string resolvedPath,
    out string errorMessage)
```

Checks an explicit path, the current directory, and the system `PATH`, returning an absolute path when FFmpeg is found. `Form1` uses this before disabling the START button, so a missing FFmpeg installation is reported with a message box instead of surfacing as a background-thread crash. `StreamVideos()` performs the same check as a defensive measure for non-UI callers.

## Events

### `ErrorOccurred`

```csharp
public event EventHandler<VideoStreamerErrorEventArgs> ErrorOccurred;
```

Raised when the folder cannot be scanned, FFmpeg cannot be started, or FFmpeg exits with a non-zero exit code. The event includes the message, input file, optional exit code, and optional exception. The message is also sent to `Trace`, and the WinForms client displays a shortened version in `StatusLabel`.

## Public methods

### `StartStreaming()`

```csharp
public void StartStreaming()
```

Starts the streaming loop on a dedicated background thread.

- If a thread is already running (`_streamThread != null && _streamThread.IsAlive`), this is a no-op.
- Otherwise, resets the stop signal and starts a background thread targeting `StreamVideos()`.
- FFmpeg path and folder errors are reported through `ErrorOccurred`; they do not escape from the worker and terminate the application.

### `StopStreaming()`

```csharp
public void StopStreaming()
```

Requests the loop to stop, signals its wait handle, and force-terminates the active FFmpeg process. The worker is joined briefly so disposal does not leave an old process racing a new stream. Stopping suppresses the non-zero exit diagnostic that naturally results from killing FFmpeg.

### `Dispose()`

```csharp
public void Dispose()
```

Stops the worker and releases its cancellation signal. `Form1` calls this when STOP is clicked and when the window closes.

## Private methods

### `StreamVideos()`

```csharp
private void StreamVideos()
```

The method executed on the background thread:

1. Validates that the configured FFmpeg executable can be resolved.
2. Recursively lists files under `_videoFolder` (`SearchOption.AllDirectories`) whose extension (case-insensitively) is one of `_videoExtensions`.
3. Reports an empty or inaccessible folder through `ErrorOccurred` and waits for a stop or a later scan. This also allows a video added after START to be picked up.
4. Sorts the current snapshot by filename, or applies a Fisher-Yates shuffle when `_shuffle` is enabled.
5. Runs each file once, skipping paths removed after the scan.
6. Repeats from step 2 after the pass completes, so additions and removals are reflected without restarting the stream.

### `RunFfmpeg`

```csharp
private void RunFfmpeg(string inputFile, string ffmpegPath)
```

Builds and runs a single FFmpeg invocation for one file:

```csharp
var args = $"-re -i \"{inputFile}\" -c copy -f flv \"{_rtmpUrl}\"";
```

| Flag | Meaning |
|---|---|
| `-re` | Read input at its native frame rate, which is required for live-streaming (rather than FFmpeg processing the file as fast as possible). |
| `-i "<inputFile>"` | The current video file. |
| `-c copy` | Stream copy — remux without re-encoding either the video or audio stream, keeping CPU usage low. |
| `-f flv` | Force FLV container/muxer, required for RTMP. |
| `"<rtmpUrl>"` | The destination RTMP URL (server + stream key). |

Each file is played once by FFmpeg. The outer playlist loop provides the continuous replay, so a file is not accidentally played twice per pass.

Execution details:

- Uses `ProcessStartInfo` with `UseShellExecute = false`, `RedirectStandardOutput = true`, `RedirectStandardError = true`, and `CreateNoWindow = true`.
- Drains FFmpeg output asynchronously and collects stderr for a failed invocation. A non-zero exit code is sent through `ErrorOccurred` with the FFmpeg diagnostic text.
- The started `Process` is stored in `_ffmpegProcess` so that `StopStreaming()` can kill it.
- `process.WaitForExit()` blocks only the background thread until FFmpeg exits; it does not block the WinForms UI.
- The `Process` object is wrapped in a `using` block, ensuring its handles are released once it exits.
