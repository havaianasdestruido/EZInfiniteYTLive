---
id: faq
title: FAQ & Troubleshooting
sidebar_position: 9
---

# FAQ & Troubleshooting

### Does this work on macOS or Linux?

No. The UI is built with Windows Forms (`System.Windows.Forms`) targeting .NET Framework 4.8.1, which only runs on Windows. See [Installation](./getting-started/installation.md).

### Nothing happens when I click START / the stream never appears live

Check, in order:

1. **Is FFmpeg actually installed and on `PATH`?** Open a terminal and run `ffmpeg -version`. The app invokes FFmpeg as plain `ffmpeg.exe` (see [`Form1._ffmpegPath`](./api-reference/form1.md#fields)), so if it isn't resolvable on `PATH`, starting the process throws an unhandled exception on the background thread, which can terminate the whole app rather than failing quietly (see [Known Limitations](./api-reference/video-streamer.md#known-limitations)).
2. **Did you select a folder that actually contains supported video files?** Only `.mp4`, `.mkv`, `.avi`, `.mov`, and `.flv` files directly inside the selected folder (not subfolders) are picked up.
3. **Are the RTMP URL and stream key correct?** Double check for trailing spaces or an incorrect/expired stream key from your platform's dashboard.
4. **Is your stream key valid for more than a few minutes?** Some platforms rotate/expire stream keys; regenerate one if needed.

### I selected "RandOrder?" but videos still play alphabetically

This is a known limitation in the current codebase — the shuffle flag on `Form1` isn't yet wired into `VideoStreamer`'s playback logic. See [Known Limitations](./api-reference/video-streamer.md#known-limitations) and consider [contributing](./contributing.md) a fix.

### Can I use this for Twitch / Kick / a self-hosted RTMP server instead of YouTube?

Yes — just replace the default RTMP URL (`rtmp://a.rtmp.youtube.com/live2`) with your platform's RTMP ingest URL and paste in the matching stream key. See [Usage Guide](./getting-started/usage.md#choosing-an-rtmp-destination).

### My video plays but the quality/audio seems off

The app streams with `-c copy` (no re-encoding) — see [`RunFfmpeg`](./api-reference/video-streamer.md#runffmpegstring-inputfile). This means the original file's codec, resolution, and bitrate are sent as-is. If your source files use a codec that FFmpeg can't cleanly remux to FLV/RTMP (e.g. unusual audio codecs), you may see corrupted playback or FFmpeg may fail for that file. Re-encode problematic files to H.264/AAC beforehand (outside of this app, e.g. with a plain FFmpeg command) for best compatibility.

### How do I stop the stream without closing the app?

Click **STOP** (`ForceStopButton`). This immediately kills the running FFmpeg process rather than waiting for the current file to finish. See [`VideoStreamer.StopStreaming()`](./api-reference/video-streamer.md#stopstreaming).

### Does closing the app stop the stream?

Yes — `Form1` disposes the active `VideoStreamer` (which stops streaming) in its `FormClosed` event handler.

### Where are the embedded sample videos?

Four small `.mkv` sample files ship in `EZInfiniteYTLive/Resources/examples/` (`1.mkv`, `2.mkv`, `3.mkv`, `sample.mkv`) and are copied next to the built executable. They're handy for a quick smoke test of the folder-scanning/looping behavior without needing your own media — point **Choose Videos** at that folder.

### Is there a way to change which codecs/bitrate are used?

Not currently — the FFmpeg arguments (`-re -stream_loop 1 -i ... -c copy -f flv ...`) are hardcoded in [`VideoStreamer.RunFfmpeg`](./api-reference/video-streamer.md#runffmpegstring-inputfile). Changing them requires editing the source and rebuilding (see [Building From Source](./building-from-source.md)), or contributing a UI option for it (see [Contributing](./contributing.md)).

### Didn't find your answer?

Open an issue on the [GitHub repository](https://github.com/havaianasdestruido/EZInfiniteYTLive/issues).
