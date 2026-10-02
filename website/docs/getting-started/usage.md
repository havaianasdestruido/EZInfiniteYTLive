---
id: usage
title: Usage Guide
sidebar_position: 2
---

# Usage Guide

This page walks through using the EZInfiniteYTLive GUI to start a 24/7 looping stream.

## The main window

The application window exposes the following controls (see [`Form1.Designer.cs`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/Form1.Designer.cs) for the exact layout):

| Control | Name in code | Purpose |
|---|---|---|
| RTMP URL textbox | `RMTPUrl` | Base RTMP server URL. Defaults to `rtmp://a.rtmp.youtube.com/live2` (YouTube's standard ingest endpoint). |
| Stream Key textbox | `StreamKey` | Your stream key for the destination service (masked with `*` since it's a secret). |
| "Choose Videos" button | `ChooseVideosButton` | Opens a folder picker to select the directory containing your video files. |
| "START" button | `StartButton` | Begins streaming the chosen folder in a loop. |
| "STOP" button | `ForceStopButton` | Immediately stops streaming and kills the running FFmpeg process. |
| "RandOrder?" radio | `ShuffleRadio` | Selects **shuffle/random** playback order. |
| "AlphabeticOrder?" radio | `AlphabeticOrderRadio` | Selects **alphabetic** playback order (checked by default). |
| Status label | `StatusLabel` | Shows the current status (`Idle`, `Selected: <path>`, `Streaming...`, `Stopped`). |

## Step-by-step

### 1. Choose your videos

Click **Choose Videos** and select the folder that contains the video files you want to loop. The folder can contain any mix of the following extensions (defined in [`VideoStreamer.cs`](../api-reference/video-streamer.md)):

```
.mp4  .mkv  .avi  .mov  .flv
```

The status label updates to show the selected path, e.g. `Selected: C:\Videos\LoopFolder`.

:::tip
Files are matched purely by name within the folder (non-recursive — subfolders are not scanned).
:::

### 2. Pick a playback order

- **AlphabeticOrder?** (default) — files are streamed in ascending alphabetical order by filename, then wraps back to the first file.
- **RandOrder?** — enables shuffle mode.

:::note Current build behavior
In the current codebase, the `_shuffle` flag is tracked on `Form1`, but `VideoStreamer.StreamVideos()` always sorts files alphabetically (`OrderBy(f => f)`) before looping — the shuffle flag is not yet wired into `VideoStreamer`. See [`video-streamer.md`](../api-reference/video-streamer.md#known-limitations) for details. Keep this in mind if you select **RandOrder?** and still observe alphabetic playback.
:::

### 3. Enter your RTMP URL and stream key

- **RTMP URL**: the ingest server address. For YouTube Live this is pre-filled as `rtmp://a.rtmp.youtube.com/live2`. For other platforms (Twitch, Kick, a self-hosted `nginx-rtmp` server, etc.), replace it with that platform's RTMP ingest URL.
- **Stream Key**: the secret key issued by your streaming platform for your channel/stream. Treat this like a password — anyone with it can stream to your channel.

The app concatenates these into a single RTMP target: `"<RTMP URL>/<Stream Key>"` (adding a `/` only if the URL doesn't already end with one).

### 4. Start streaming

Click **START**. The app:

1. Validates that a folder was selected and that it still exists.
2. Validates that both the RTMP URL and stream key are non-empty.
3. Builds the full RTMP destination URL.
4. Creates a `VideoStreamer` and calls `StartStreaming()`, which spins up a background thread that runs FFmpeg against each video file in sequence.
5. Updates the window title and status label to `Streaming...`, disables **START**, and enables **STOP**.

### 5. Stop streaming

Click **STOP** (`ForceStopButton`) at any time to:

1. Signal the background loop to stop.
2. Kill the in-flight FFmpeg process (so there's no "waiting for the current file to end" delay).
3. Dispose of the `VideoStreamer` instance.
4. Reset the status label/title to `Stopped` and re-enable **START**.

Closing the window also stops and disposes of any active stream automatically (`Form1`'s `FormClosed` handler).

## Choosing an RTMP destination

Any RTMP(S)-compatible service works. A few examples:

| Platform | Example RTMP URL |
|---|---|
| YouTube Live | `rtmp://a.rtmp.youtube.com/live2` |
| Twitch | `rtmp://live.twitch.tv/app` |
| Kick | Provided in your Kick stream dashboard |
| Self-hosted (e.g. `nginx-rtmp`) | `rtmp://your-server-ip/live` |

## Tips for reliable 24/7 streaming

- Prefer source files already encoded as **H.264 video / AAC audio** inside a container FFmpeg can remux cleanly to FLV (the app streams with `-c copy`, i.e. no re-encoding — see [`RunFfmpeg`](../api-reference/video-streamer.md#runffmpegstring-inputfile)). Mixed/incompatible codecs across files can cause FFmpeg to fail for some files in the folder.
- Keep the machine running the app online and awake — if the process/computer stops, the stream stops.
- Because `-c copy` is used, the video resolution/framerate of each file determines the live stream's characteristics for its duration; wildly inconsistent source files may cause hiccups on some platforms (YouTube generally tolerates this well).
- Test with a short clip and a private/unlisted stream first before going live publicly.
