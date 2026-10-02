---
id: configuration
title: Configuration
sidebar_position: 5
---

# Configuration

EZInfiniteYTLive is deliberately config-light: almost everything is set through the UI at runtime rather than through config files. This page documents the configuration surfaces that do exist.

## `App.config`

**File:** [`EZInfiniteYTLive/App.config`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/App.config)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8.1" />
    </startup>
</configuration>
```

This declares the supported CLR runtime for the compiled executable. It only affects which .NET Framework runtime loads the app — it is **not** a place for application settings in the current codebase.

## `EZInfiniteYTLive.csproj` build settings

**File:** [`EZInfiniteYTLive/EZInfiniteYTLive.csproj`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/EZInfiniteYTLive.csproj)

Key properties:

| Property | Value | Meaning |
|---|---|---|
| `OutputType` | `WinExe` | Produces a windowed (non-console) executable. |
| `TargetFrameworkVersion` | `v4.8.1` | Requires the .NET Framework 4.8.1 runtime. |
| `RootNamespace` / `AssemblyName` | `EZInfiniteYTLive` | |

The project also defines build configurations for multiple platforms: `AnyCPU`, `ARM64`, `x64`, and `x86`, each with `Debug`/`Release` variants (see [Building From Source](./building-from-source.md)).

Embedded resources (`Resources\examples\*.mkv`) are copied to the output directory (`CopyToOutputDirectory: Always`) so sample videos ship alongside the built executable.

## FFmpeg path

The path used to invoke FFmpeg is **hardcoded** in [`Form1.cs`](./api-reference/form1.md):

```csharp
private string _ffmpegPath = "ffmpeg.exe"; // Adjust if needed
```

There is currently **no UI field or config file setting** to change this at runtime. To point the app at a specific FFmpeg binary (e.g. a portable copy not on `PATH`), you must edit this line and rebuild — see [Contributing](./contributing.md) if you'd like to turn this into a user-configurable setting (e.g. backed by `Properties/Settings.settings`, which is already scaffolded into the project but not yet used).

## Default RTMP URL

The RTMP URL textbox (`RMTPUrl`) is pre-populated in `Form1.Designer.cs` with:

```
rtmp://a.rtmp.youtube.com/live2
```

This is just the default `Text` of the textbox — it's fully editable at runtime and not persisted between runs (no settings file currently stores it).

## `Properties/Settings.settings`

Visual Studio scaffolds a `Settings.settings` / `Settings.Designer.cs` pair for every WinForms project by default, which would normally back a `Settings.settings` → `app.config`-persisted user settings system (`Properties.Settings.Default.*`). In this codebase, **no settings are currently defined or consumed** — it's an empty scaffold left over from the project template, and a natural place to add persistence for things like the last-used video folder, FFmpeg path, or last RTMP URL in a future contribution.

## Supported video extensions

Defined as a constant array in [`VideoStreamer.cs`](./api-reference/video-streamer.md):

```csharp
private readonly string[] _videoExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".flv" };
```

This is not currently configurable without editing source — files with other extensions in the selected folder are silently ignored.
