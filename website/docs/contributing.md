---
id: contributing
title: Contributing
sidebar_position: 8
---

# Contributing

EZInfiniteYTLive is a small project, which makes it a great candidate for first-time contributions. This page covers the practical workflow; see [Architecture Overview](./architecture/overview.md) and [API Reference](./api-reference/program.md) first to understand the codebase.

## Getting set up

1. Fork and clone the repository.
2. Follow [Installation](./getting-started/installation.md) / [Building From Source](./building-from-source.md) to get a local build running.
3. Make sure FFmpeg is installed so you can actually test streaming (e.g. to a private/unlisted YouTube stream, or a local RTMP server such as [`nginx-rtmp`](https://github.com/arut/nginx-rtmp-module) or [MediaMTX](https://github.com/bluenviron/mediamtx)).

## Suggested first contributions

Based on the [Known Limitations](./api-reference/video-streamer.md#known-limitations) of `VideoStreamer` and the [CI/CD gaps](./ci-cd.md#what-the-pipeline-does-not-currently-do):

- **Wire up shuffle mode.** Pass `Form1._shuffle` into `VideoStreamer`'s constructor and use it inside `StreamVideos()` to randomize file order (e.g. shuffle once per full pass, or re-shuffle every cycle).
- **Surface FFmpeg errors in the UI.** Read the redirected stdout/stderr in `RunFfmpeg` and show failures (e.g. in `StatusLabel` or a log panel) instead of silently moving to the next file.
- **Persist settings.** Use the already-scaffolded `Properties/Settings.settings` to remember the last-used video folder, RTMP URL, and FFmpeg path between sessions.
- **Make the FFmpeg path configurable from the UI**, rather than hardcoded in `Form1._ffmpegPath`.
- **Add a unit test project** for pure-logic pieces (e.g. file discovery/sorting logic extracted from `VideoStreamer.StreamVideos`) and wire it into the GitHub Actions workflow.
- **Add recursive folder scanning** as an opt-in toggle.
- **Publish build artifacts** from CI (e.g. via `actions/upload-artifact`) so testers can grab a build without compiling locally.

## Code style notes

- The codebase mixes English and Portuguese comments/strings (e.g. `Program.cs`'s `/// <summary>` comment and `Form1.Designer.cs`'s `#region` are in Portuguese, reflecting Visual Studio's localized designer templates). New code can be written in English; there's no strict enforced style guide, so prefer matching the existing conventions in the file you're editing (naming like `PascalCase` for public members/controls, `_camelCase` for private fields).
- UI changes to `Form1` should be made via the Visual Studio **Windows Forms Designer** when possible, since `Form1.Designer.cs` is machine-generated and easy to desync from the `.resx`/designer view if hand-edited carelessly.

## Submitting changes

1. Create a branch for your change.
2. Make sure the solution builds (`msbuild EZInfiniteYTLive.sln /p:Configuration=Release`, or build in Visual Studio) — this mirrors what [CI](./ci-cd.md) checks.
3. Test the behavior manually using a short sample clip (the repo already embeds sample files under `EZInfiniteYTLive/Resources/examples/`) streamed to a private/unlisted or local RTMP endpoint.
4. Open a pull request against `main` describing the change and how you tested it.

## Documentation contributions

This documentation site is built with [Docusaurus](https://docusaurus.io/) and lives in the `website/` directory at the root of the repository. To work on the docs locally:

```bash
cd website
npm install
npm start
```

This starts a local dev server (default `http://localhost:3000`) with hot reload. Docs content lives under `website/docs/` as Markdown/MDX files; see Docusaurus's [documentation guide](https://docusaurus.io/docs/create-doc) for syntax and conventions (front matter, `sidebar_position`, Mermaid diagrams, etc.).

To produce a static production build:

```bash
npm run build
```

The output is written to `website/build/` and can be served with `npm run serve` or deployed to any static host (GitHub Pages, Netlify, Vercel, etc.).
