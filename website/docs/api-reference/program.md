---
id: program
title: Program
sidebar_position: 1
---

# `Program` class

**File:** [`EZInfiniteYTLive/Program.cs`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/EZInfiniteYTLive/Program.cs)
**Namespace:** `EZInfiniteYTLive`
**Type:** `internal static class`

The application's entry point. This is standard, mostly auto-generated WinForms bootstrap code.

```csharp
namespace EZInfiniteYTLive
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
```

## Methods

### `Main()`

```csharp
[STAThread]
static void Main()
```

The main entry point for the application (`"Ponto de entrada principal para o aplicativo"` — Portuguese for *"Main entry point for the application"*).

- **`[STAThread]`** — marks the thread as Single-Threaded Apartment, required for WinForms/COM interop (clipboard, drag-and-drop, common dialogs like `FolderBrowserDialog`).
- **`Application.EnableVisualStyles()`** — enables visual styles (theming) for Windows common controls, so the UI matches the current Windows theme rather than rendering as classic/unstyled controls.
- **`Application.SetCompatibleTextRenderingDefault(false)`** — configures text rendering to use GDI+ (`TextRenderer`/GDI) instead of the older compatible mode, recommended default for modern WinForms apps.
- **`Application.Run(new Form1())`** — creates the main window ([`Form1`](./form1.md)) and starts the Windows message loop, blocking until the form is closed.

## Responsibilities

- Owns process startup only. It has **no** business logic — all UI and streaming behavior lives in [`Form1`](./form1.md) and [`VideoStreamer`](./video-streamer.md) respectively.
- There is exactly one entry point and one top-level window; the app is not designed to run headless or accept CLI arguments.
