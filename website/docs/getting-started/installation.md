---
id: installation
title: Installation
sidebar_position: 1
---

# Installation

EZInfiniteYTLive is a Windows Forms (.NET Framework) desktop application. This page covers both running a prebuilt binary and building the project yourself.

## Requirements

| Requirement | Details |
|---|---|
| **Operating System** | Windows (the app targets WinForms / .NET Framework, which is Windows-only). |
| **.NET Framework** | **4.8.1** — set via `TargetFrameworkVersion` in [`EZInfiniteYTLive.csproj`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/EZInfiniteYTLive.csproj). Install the [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework) if you intend to build from source. |
| **FFmpeg** | **Required at runtime.** The app shells out to `ffmpeg.exe` to actually encode/stream video. It must be available on your system `PATH` (the default FFmpeg installers / `winget install ffmpeg` add it to `PATH` automatically). See [Configuration](../configuration.md) for how the path is resolved. |
| **Build tooling (optional)** | Visual Studio 2019/2022 (with the ".NET desktop development" workload) or MSBuild + NuGet CLI if you prefer the command line. |

:::info Why is this Windows-only?
The UI is built with **Windows Forms** (`System.Windows.Forms`), which only runs on Windows (classic .NET Framework or .NET with the `Microsoft.WindowsDesktop.App.WindowsForms` runtime). There is currently no cross-platform build.
:::

## Option 1 — Download a release

Check the project's [GitHub Releases](https://github.com/havaianasdestruido/EZInfiniteYTLive/releases) page (if available) for a prebuilt `EZInfiniteYTLive.exe`. Simply:

1. Make sure FFmpeg is installed and on your `PATH` (run `ffmpeg -version` in a terminal to confirm).
2. Download and extract the release.
3. Run `EZInfiniteYTLive.exe`.

## Option 2 — Build from source

### 1. Clone the repository

```bash
git clone https://github.com/havaianasdestruido/EZInfiniteYTLive.git
cd EZInfiniteYTLive
```

### 2. Restore NuGet packages

```bash
nuget restore EZInfiniteYTLive.sln
```

### 3. Build with MSBuild

```bash
msbuild EZInfiniteYTLive.sln /p:Configuration=Release
```

This mirrors exactly what the project's CI pipeline does — see [`.github/workflows/dotnet.yml`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/.github/workflows/dotnet.yml) and the [CI/CD page](../ci-cd.md) for details.

Alternatively, open `EZInfiniteYTLive.sln` in Visual Studio and build/run from there (<kbd>F5</kbd> or <kbd>Ctrl+Shift+B</kbd>).

### 4. Locate the executable

After a successful build, the executable is produced at:

```
EZInfiniteYTLive/bin/Release/EZInfiniteYTLive.exe
```

(or `bin/Debug/...` for a Debug build).

### 5. Install FFmpeg

Download FFmpeg from [ffmpeg.org](https://ffmpeg.org/download.html) (or install it via a package manager, e.g. `winget install ffmpeg` / `choco install ffmpeg`) and make sure the `ffmpeg` executable is resolvable from `PATH`, since the app invokes it simply as `ffmpeg.exe` (see [`Form1.cs`](../api-reference/form1.md)).

Verify with:

```bash
ffmpeg -version
```

## Next steps

Once FFmpeg is installed and the app is built/downloaded, head to the [Usage Guide](./usage.md) to start your first stream.
