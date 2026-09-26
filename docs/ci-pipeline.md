# CI Pipeline

The CI pipeline uses GitHub Actions to invoke the [`ci.yml`](../.github/workflows/ci.yml) workflow. At the last step the [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj) project is run which contains the majority of CI. This project uses the [ModularPipelines](https://thomhurst.github.io/ModularPipelines/docs/) package.

<!-- Not for AI agents: ensure to always use the [documents](https://thomhurst.github.io/ModularPipelines/llms.txt) -->

There's no conditional registration of services in the pipeline. The whole thing is built on every run with.

---

## [`ci.yml`](../.github/workflows/ci.yml)

- Ignores non-relevant paths such as document changes and more
- Cancels previous running workflows on push
- Runs the latest Windows Server

### Steps

1. Disables Windows Defender on host machine
2. Checkout
3. .NET setup
4. Runs [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj) project

---

## [`WhoHolds.Pipeline`](../src/WhoHolds.Pipeline/WhoHolds.Pipeline.csproj)

### Requirements

|Step| Requirement | Runs | Description |
|-|-|-|- |
|1.| `WindowsRequirement` | Always | The pipeline must run on Windows. |
|2.| [`VersionTagFormatRequirement`](../src/WhoHolds.Pipeline/Requirements/VersionTagFormatRequirement.cs) | On tag push | Verifies git tag name by 'GITHUB_REF_NAME' using a source generated regex. The tag name must have the following format: vMAJOR.MINOR.PATCH (e.g. v1.0.0).
|3.| [`CppBuildToolsRequirement`](../src/WhoHolds.Pipeline/Requirements/CppBuildToolsRequirement.cs) | On tag push | Ensures that Desktop Development with C++ is installed on host. |

**`IPipelineRequirement` has no skip mechanic like `Module` does so returning early is the right choice for conditional requirements.**

### Modules

|Step| Module | Depends on | Runs | Description |
|-|-|-|- | - |
| 1. | [`RestoreModule`](../src/WhoHolds.Pipeline/Modules/RestoreModule.cs) | - | Always | Restores NuGet packages. |
| 2. | [`BuildModule`](../src/WhoHolds.Pipeline/Modules/BuildModule.cs) | `RestoreModule` | Always | Builds the [Solution](../WhoHolds.slnx)
| 3. | [`TestModule`](../src/WhoHolds.Pipeline/Modules/TestModule.cs) | `BuildModule` | Always | Runs all the tests from the build output.
| 4. | [`PublishModule`](../src/WhoHolds.Pipeline/Modules/PublishModule.cs) | `TestModule` | On tag push | Builds native AOT exe: [WhoHolds.Cli](../src/WhoHolds.Cli).

**Module skipping is defined in `Configure` method override.**