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

1. Enumerating supported video files in a folder.
2. Running FFmpeg against each file, one at a time, targeting an RTMP URL.
3. Looping back to the first file once the last one finishes, forever, until stopped.

## Fields

| Field | Type | Description |
|---|---|---|
| `_ffmpegPath` | `readonly string` | Path or command name used to launch FFmpeg (e.g. `"ffmpeg.exe"`). |
| `_videoFolder` | `readonly string` | Folder to scan for video files. |
| `_rtmpUrl` | `readonly string` | Full destination RTMP URL (server + stream key). |
| `_videoExtensions` | `readonly string[]` | Supported file extensions: `.mp4`, `.mkv`, `.avi`, `.mov`, `.flv`. |
| `_streamThread` | `Thread` | The background thread running the infinite streaming loop. |
| `_stopRequested` | `bool` | Cooperative cancellation flag checked by the streaming loop. |
| `_ffmpegProcess` | `Process` | Reference to the currently running FFmpeg process, so it can be killed on demand. |

## Constructor

### `VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl)`

```csharp
public VideoStreamer(string ffmpegPath, string videoFolder, string rtmpUrl)
```

Stores the three required pieces of configuration. Performs no validation or I/O itself — validation happens in the caller ([`Form1.StartButton_Click`](./form1.md#startbutton_click)).

| Parameter | Description |
|---|---|
| `ffmpegPath` | Executable name/path for FFmpeg. |
| `videoFolder` | Directory containing the video files to stream. |
| `rtmpUrl` | Fully-formed RTMP destination, e.g. `rtmp://a.rtmp.youtube.com/live2/<stream-key>`. |

## Public methods

### `StartStreaming()`

```csharp
public void StartStreaming()
```

Starts the infinite streaming loop on a dedicated background thread.

- If a thread is already running (`_streamThread != null && _streamThread.IsAlive`), this is a no-op — calling `StartStreaming()` twice on the same instance without stopping first has no additional effect.
- Otherwise, resets `_stopRequested = false` and starts a new `Thread` targeting `StreamVideos()`, marked `IsBackground = true` (so it won't keep the process alive on its own if the app tries to exit).

### `StopStreaming()`

```csharp
public void StopStreaming()
```

Requests the loop to stop and force-terminates the active FFmpeg process:

```csharp
_stopRequested = true;
try
{
    if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
    {
        _ffmpegProcess.Kill();
    }
}
catch { }
```

- Setting `_stopRequested = true` requests that the loop exit after the current `RunFfmpeg` call returns — it does **not** prevent an iteration that already passed the `while (!_stopRequested)` check from starting a new `RunFfmpeg`/FFmpeg process.
- Killing the process is what makes stopping **immediate** rather than waiting for the current file to finish playing — without this, `WaitForExit()` inside `RunFfmpeg` would block until the file naturally ends. However, this only takes effect once `_ffmpegProcess` has been (re)assigned to the process that's actually running; if `StopStreaming()` is called in the brief window after a loop iteration starts but before `RunFfmpeg` has assigned the new `Process` to `_ffmpegProcess`, the kill can miss it (acting on the previous, already-exited process instead), and that file will run to completion before the loop observes `_stopRequested` on its next check.
- Exceptions from `Kill()` (e.g. the process already exited in a race) are intentionally swallowed.

### `Dispose()`

```csharp
public void Dispose()
{
    StopStreaming();
}
```

Implements `IDisposable` by delegating to `StopStreaming()`, so `VideoStreamer` instances can be safely cleaned up with a `using` block or explicit `Dispose()` call (as `Form1` does).

## Private methods

### `StreamVideos()`

```csharp
private void StreamVideos()
```

The method executed on the background thread (`_streamThread`):

1. Lists files directly inside `_videoFolder` (non-recursive) whose extension (case-insensitively) is one of `_videoExtensions`.
2. Sorts them with `OrderBy(f => f)` — i.e. **alphabetically by full path**.
3. If no matching files are found, returns immediately (no stream starts).
4. Otherwise, loops `while (!_stopRequested)`:
   - Plays the file at the current index via `RunFfmpeg(file)`.
   - Advances `idx = (idx + 1) % files.Count`, wrapping back to `0` after the last file — this is what makes the stream "infinite".

### `RunFfmpeg(string inputFile)`

```csharp
private void RunFfmpeg(string inputFile)
```

Builds and runs a single FFmpeg invocation for one file:

```csharp
var args = $"-re -stream_loop 1 -i \"{inputFile}\" -c copy -f flv \"{_rtmpUrl}\"";
```

| Flag | Meaning |
|---|---|
| `-re` | Read input at its native frame rate, which is required for live-streaming (rather than FFmpeg processing the file as fast as possible). |
| `-stream_loop 1` | Loop the single input file one extra time (i.e. play it twice) before FFmpeg's own process exits. |
| `-i "<inputFile>"` | The current video file. |
| `-c copy` | Stream copy — remux without re-encoding either the video or audio stream, keeping CPU usage low. |
| `-f flv` | Force FLV container/muxer, required for RTMP. |
| `"<rtmpUrl>"` | The destination RTMP URL (server + stream key). |

Execution details:

- Uses `ProcessStartInfo` with `UseShellExecute = false`, `RedirectStandardOutput = true`, `RedirectStandardError = true`, and `CreateNoWindow = true` — i.e. FFmpeg runs hidden, with its stdout/stderr redirected (and read asynchronously via `BeginOutputReadLine()` / `BeginErrorReadLine()`, though the output isn't currently logged anywhere by the app).
- The started `Process` is stored in `_ffmpegProcess` so that `StopStreaming()` can kill it.
- `process.WaitForExit()` blocks the background thread until FFmpeg exits (either because the file finished playing twice, or because it was killed by `StopStreaming()`).
- The `Process` object is wrapped in a `using` block, ensuring its handles are released once it exits.

## Known limitations

These are useful starting points if you want to [contribute](../contributing.md):

1. **Shuffle is not implemented in `VideoStreamer`.** `Form1._shuffle` is tracked but never passed to `VideoStreamer`'s constructor or used in `StreamVideos()` — files are always played in alphabetical order (`OrderBy(f => f)`). To honor "RandOrder?", `VideoStreamer` would need to accept a shuffle flag and (e.g.) use `OrderBy(f => Guid.NewGuid())` or a `Random`-based shuffle instead of/alongside the alphabetic sort.
2. **No folder change detection.** The file list is captured once per `StreamVideos()` call (i.e., once per `StartStreaming()` call); adding/removing files from the folder mid-stream has no effect until the stream is stopped and restarted.
3. **No logging/surfacing of FFmpeg errors.** `RunFfmpeg` redirects FFmpeg's stdout/stderr but doesn't log, display, or otherwise react to it — if FFmpeg fails immediately for a given file (e.g. unsupported codec), the loop simply advances to the next file with no user-visible diagnostic.
4. **`-stream_loop 1` plays each file twice per cycle.** Combined with the outer `while` loop in `StreamVideos()`, each file is effectively played twice before the file list advances. This may or may not be intentional; worth confirming against your expected behavior.
5. **Non-recursive folder scan.** Subfolders of the selected directory are ignored.
6. **No FFmpeg path validation.** If FFmpeg isn't installed or isn't on `PATH`, `Process.Start()` will throw a `Win32Exception`, which is not currently caught around `RunFfmpeg`/`StreamVideos`. Since there's no global unhandled-exception handling in `Program`/`Form1`, this exception on the background thread can terminate the entire application rather than failing with a user-facing error message.
