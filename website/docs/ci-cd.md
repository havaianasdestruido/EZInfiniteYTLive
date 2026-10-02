---
id: ci-cd
title: CI/CD
sidebar_position: 7
---

# Continuous Integration

**File:** [`.github/workflows/dotnet.yml`](https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/.github/workflows/dotnet.yml)

The repository uses a single GitHub Actions workflow, named **".NET Framework"**, to validate that the solution builds successfully.

## Triggers

```yaml
on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]
```

The workflow runs on every push to `main` and on every pull request targeting `main`.

## Job: `build`

```yaml
jobs:
  build:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v4
    - name: Restore NuGet packages
      run: nuget restore EZInfiniteYTLive.sln
    - uses: microsoft/setup-msbuild@v2
    - name: Build with MSBuild
      run: msbuild EZInfiniteYTLive.sln /p:Configuration=Release
    # Add your test step here as needed
```

Runs on a **`windows-latest`** runner (required, since the project is a WinForms/.NET Framework app) and performs:

1. **Checkout** — `actions/checkout@v4` clones the repository.
2. **Restore** — `nuget restore EZInfiniteYTLive.sln` downloads NuGet dependencies declared by the solution/project.
3. **Set up MSBuild** — `microsoft/setup-msbuild@v2` locates and adds `msbuild.exe` to the runner's `PATH`.
4. **Build** — `msbuild EZInfiniteYTLive.sln /p:Configuration=Release` compiles the solution in `Release` configuration (default `AnyCPU` platform, since none is specified).

## What the pipeline does *not* currently do

- **No automated tests** — the workflow comment explicitly notes: `# Add your test step here as needed`. There is currently no test project in the solution.
- **No artifact publishing** — the built `.exe` is not uploaded as a workflow artifact or attached to a release.
- **No linting/static analysis** step.
- **No multi-platform matrix build** — only the default `AnyCPU` platform is built, even though the `.csproj` defines `x86`/`x64`/`ARM64` configurations.

These are reasonable areas to improve if you're looking to [contribute](./contributing.md) — e.g. adding an `actions/upload-artifact` step to publish the built executable, or adding a unit test project for `VideoStreamer`'s file-discovery logic.

## Reproducing CI locally

Since the workflow is just two commands, you can reproduce it on a Windows machine (or Windows-hosted runner) with the [Developer Command Prompt for VS](https://learn.microsoft.com/en-us/visualstudio/ide/reference/command-prompt-powershell?view=vs-2022) or any shell with `msbuild`/`nuget` on `PATH`:

```bash
nuget restore EZInfiniteYTLive.sln
msbuild EZInfiniteYTLive.sln /p:Configuration=Release
```

See [Building From Source](./building-from-source.md) for more detail.
