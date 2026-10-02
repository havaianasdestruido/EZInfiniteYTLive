---
id: intro
title: Introduction
sidebar_position: 1
---

# EZInfiniteYTLive

**EZInfiniteYTLive** is a small, easy-to-use Windows desktop tool for running **24/7 looping livestreams** to YouTube Live or any other RTMP-compatible streaming service (Twitch, Kick, self-hosted RTMP servers, etc.).

It wraps [FFmpeg](https://ffmpeg.org/) with a minimal WinForms GUI: point it at a folder of video files, give it an RTMP URL and stream key, and it will stream the videos in that folder back-to-back, forever, restarting from the beginning once the folder has been played through.

![EZInfiniteYTLive main window](https://github.com/user-attachments/assets/5bd24a48-c353-47a0-8ef9-0e1af8d9a16b)

## Why does this exist?

Many "24/7 stream" / "infinite loop" YouTube channels (lofi radios, ambient loops, looped movies/shows, etc.) are powered by exactly this pattern: take a folder of video files and continuously re-stream them to an RTMP endpoint with FFmpeg. EZInfiniteYTLive packages that pattern into a simple GUI so you don't need to hand-write FFmpeg command lines or shell scripts.

## Key features

- 🎥 **Folder-based playlist** — just pick a folder, no manual playlist file needed.
- 🔁 **Infinite looping** — once the last video finishes, the first one starts again, indefinitely.
- 🔀 **Playback order control** — play files in **alphabetic order** or in **random (shuffled) order**.
- 📡 **Any RTMP destination** — not limited to YouTube; any service that accepts an RTMP(S) URL + stream key works.
- 🪶 **Lightweight** — a single WinForms executable with no heavy dependencies beyond FFmpeg.
- ⚙️ **Stream copy by default** — videos are remuxed (`-c copy`) rather than re-encoded, so CPU usage stays low and quality is preserved, provided your source files are already in an RTMP/FLV-friendly codec (e.g. H.264/AAC).

## How it works, in one paragraph

EZInfiniteYTLive scans the folder you choose for supported video files (`.mp4`, `.mkv`, `.avi`, `.mov`, `.flv`), builds an ordered (or shuffled) list of them, and then launches an FFmpeg process per file with the destination RTMP URL built from the RTMP server address and your stream key. When one file's FFmpeg process exits, it immediately starts the next one, wrapping back to the start of the list when it reaches the end — producing a continuous, infinite stream.

## Who this is for

- Creators running "lofi radio" / ambient / looped-content style 24/7 channels.
- Anyone who wants a simple, no-code way to keep a folder of clips looping to a live stream.
- Developers who want a minimal, readable reference implementation of "FFmpeg + RTMP looping" in C#.

## Where to go next

- [Installation](./getting-started/installation.md) — requirements and how to get the app running.
- [Usage Guide](./getting-started/usage.md) — walkthrough of the UI and streaming workflow.
- [Architecture Overview](./architecture/overview.md) — how the pieces fit together.
- [API Reference](./api-reference/video-streamer.md) — documentation of every class and method in the codebase.
- [Configuration](./configuration.md) — `App.config`, FFmpeg path, and other settings.
- [Contributing](./contributing.md) — how to build, test, and submit changes.

:::tip Project repository
The source code lives at [havaianasdestruido/EZInfiniteYTLive](https://github.com/havaianasdestruido/EZInfiniteYTLive) on GitHub.
:::
