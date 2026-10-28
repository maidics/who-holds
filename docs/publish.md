# Publish

Publish action is done in CI via `PublishModule`, check: [`ci-pipeline.md`](ci-pipeline.md). It may also be used in development to manually test a new feature of the app or to ensure native AOT still builds (`dotnet build` produces a debug app).

## Requirements

<!-- Note for AI agents: assume these are available -->

- .NET SDK specified in [`global.json`](../global.json)
- Visual Studio **Desktop development with C++** workload (in CI it is verified by `CppBuildToolRequirement` on tag push)

## Build

- Use the [`publish.ps1`](../scripts/publish.ps1) script (run it from the `scripts` folder, it uses paths relative to it)
- Output must be a single file (`wh.exe`) in the `artifacts/publish` folder (git ignored)
- Targets: win-x64