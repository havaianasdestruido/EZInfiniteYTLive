---
id: building-from-source
title: Building From Source
sidebar_position: 6
---

# Building From Source

This page expands on the [Installation](./getting-started/installation.md) guide with more detail on the build system itself, useful if you plan to modify the code.

## Prerequisites

- **Windows** (WinForms / .NET Framework projects build and run on Windows).
- **.NET Framework 4.8.1 Developer Pack** (or a Visual Studio installation that includes it).
- **MSBuild** — included with Visual Studio, or available standalone via the [Build Tools for Visual Studio](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022).
- **NuGet CLI** (`nuget.exe`) for restoring packages from the command line, or let Visual Studio handle it automatically.

## Solution & project layout

- **Solution:** `EZInfiniteYTLive.sln`
- **Project:** `EZInfiniteYTLive/EZInfiniteYTLive.csproj` (the only project in the solution)

## Build configurations

The `.csproj` defines build settings for the following `Configuration|Platform` combinations:

- `Debug|AnyCPU`, `Release|AnyCPU`
- `Debug|ARM64`, `Release|ARM64`
- `Debug|x64`, `Release|x64`
- `Debug|x86`, `Release|x86`

Each writes its output to a configuration/platform-specific subfolder under `bin\`, e.g. `bin\Release\`, `bin\x64\Debug\`, `bin\ARM64\Release\`, etc.

## Command-line build

From the repository root:

```bash
# 1. Restore NuGet packages referenced by the solution
nuget restore EZInfiniteYTLive.sln

# 2. Build in Release configuration (default platform: AnyCPU)
msbuild EZInfiniteYTLive.sln /p:Configuration=Release
```

To target a specific platform:

```bash
msbuild EZInfiniteYTLive.sln /p:Configuration=Release /p:Platform=x64
```

This is exactly what the project's CI workflow does — see [CI/CD](./ci-cd.md).

## Building with Visual Studio

1. Open `EZInfiniteYTLive.sln` in **Visual Studio 2022 version 17.3 or later** (required to target .NET Framework 4.8.1), with the **.NET desktop development** workload and the **.NET Framework 4.8.1 Developer Pack** installed.
2. Select a configuration/platform (e.g. `Release` / `Any CPU`) from the toolbar.
3. Build via **Build → Build Solution** (<kbd>Ctrl+Shift+B</kbd>) or run directly with <kbd>F5</kbd> / <kbd>Ctrl+F5</kbd>.

## Output

A successful build produces `EZInfiniteYTLive.exe` plus its `.config`, `.pdb` (Debug), and any copied resources (the sample `.mkv` files in `Resources/examples/`) inside the relevant `bin\<Platform>\<Configuration>\` folder.

## Running the built app

Make sure FFmpeg is installed and on `PATH` (see [Installation](./getting-started/installation.md#5-install-ffmpeg)), then run the produced `.exe` directly, or press <kbd>F5</kbd> in Visual Studio to launch it under the debugger.
