# CI Pipeline

The CI pipeline uses GitHub Actions to invoke the [`ci.yml`](../.github/workflows/ci.yml) workflow. At the last step the [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj) project is run which contains the majority of CI. This project uses the [ModularPipelines](https://thomhurst.github.io/ModularPipelines/docs/) package.

<!-- Not for AI agents: ensure to always use the [documents](https://thomhurst.github.io/ModularPipelines/llms.txt) -->

There's no conditional registration of services in the pipeline. The whole thing is built on every run.

---

## [`ci.yml`](../.github/workflows/ci.yml)

- Runs on pull requests, pushes to `main` and tag pushes
- Ignores non-relevant paths such as document changes and more (path filters are not evaluated for tag pushes)
- Cancels previous running workflows on push
- Runs the latest Windows Server

### Steps

1. Disables Windows Defender on host machine
2. Checkout
3. .NET setup
4. Runs [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj) project
5. Uploads the publish output (`artifacts/publish/`) as the `who-holds-win-x64` artifact on pull requests

---

## [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj)

Settings classes are bound from [`appsettings.Production.json`](../src/WhoHolds.Pipeline/appsettings.Production.json) (`GITHUB_REF_TYPE` and `GITHUB_REF_NAME` are read from the environment) and validated on startup via annotations.

### Requirements

| Step | Requirement                                                                                         | Runs | Description                                                                                                                                               |
|------|-----------------------------------------------------------------------------------------------------|-|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1.   | `WindowsRequirement`                                                                                | Always | The pipeline must run on Windows.                                                                                                                         |
| 2.   | [`VersionTagFormatRequirement`](../src/WhoHolds.Pipeline/Requirements/VersionTagRequirement.cs) | On tag push | Verifies git tag name by `GITHUB_REF_NAME` using a source generated regex. The tag name must have the following format: vMAJOR.MINOR.PATCH (e.g. v1.0.0). |
| 3.   | [`CppBuildToolRequirement`](../src/WhoHolds.Pipeline/Requirements/CppBuildToolRequirement.cs)       | On tag push | Ensures that Desktop Development with C++ (MSVC x64 build tools) is installed on host, located via `vswhere.exe`.                                                                                           |

**`IPipelineRequirement` has no skip mechanic like `Module` does so returning early is the right choice for conditional requirements.**

### Modules

|Step| Module | Depends on | Runs | Description |
|-|-|-|- | - |
| 1. | [`RestoreModule`](../src/WhoHolds.Pipeline/Modules/RestoreModule.cs) | - | Always | Restores NuGet packages. |
| 2. | [`BuildModule`](../src/WhoHolds.Pipeline/Modules/BuildModule.cs) | `RestoreModule` | Always | Builds the [Solution](../WhoHolds.slnx)
| 3. | [`TestModule`](../src/WhoHolds.Pipeline/Modules/TestModule.cs) | `BuildModule` | Always | Runs all the tests from the build output.
| 4. | [`PublishModule`](../src/WhoHolds.Pipeline/Modules/PublishModule.cs) | `TestModule` | Always | Builds native AOT executable: [WhoHolds.Cli](../src/WhoHolds.Cli). `Publish:OutputDirectory` is resolved once to an absolute path (against the working directory, i.e. the repository root) and passed on in `PublishedBuild`, which rejects relative paths. The version is `0.0.0-dev`, or the version resolved from the tag on tag push.
| 5. | [`SmokeTestModule`](../src/WhoHolds.Pipeline/Modules/SmokeTestModule.cs) | `PublishModule` | Always | Invokes `--version` command on the published `.exe` file by its full path. Ensures the output folder contains only one `.exe` file and its version is correct. |
| 6. | [`DraftReleaseModule`](../src/WhoHolds.Pipeline/Modules/DraftReleaseModule.cs) | `SmokeTestModule` | On tag push | Drafts a GitHub release (or reuses the existing draft for the tag, failing if it is already published) and uploads the published `.exe` artifact, replacing existing assets. |

**Module skipping is defined in `Configure` method override.**

**`ExecuteModuleAsync` and `ExecuteAsync` is glue only.** All logic lives in public static helpers so it can be unit tested. New modules should follow this pattern.