# Publish

Publish action may be used in development to manually test a new feature of the app or to ensure native AOT still builds (`dotnet build` produces a debug app).

## Requirements

<!-- Note for AI agents: assume these are available -->

- .NET SDK specified in [`global.json`](../global.json)
- Visual Studio **Desktop development with C++** workload (verify availability with [`verify-cpp-build-tools.ps1`](../scripts/verify-cpp-build-tools.ps1) script)

## Build

- Use the [`publish.ps1`](../scripts/publish.ps1) script
- Output must be a single file (`wh.exe`) in [`publish`](../artifacts/publish) folder
- Targets: win-x64