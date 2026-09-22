# Architecture

## Source

| Layer                                              | Project                                | Owns                                   | Must not leak past it                                     |
|------------------------------------|----------------------------------------|----------------------------------------|-----------------------------------------------------------|
| NativeMethods                       | [WhoHolds.Core](../src/WhoHolds.Core/) | raw signatures                         | nothing; no logic lives here                              |
| Native method handlers              | [WhoHolds.Core](../src/WhoHolds.Core/) | handle lifetime, call order            | uint handles, Win32 error codes (map to enums/ constants) |
| Orchestrator classes                                        | [WhoHolds.Core](../src/WhoHolds.Core/) | domain results              | RM_PROCESS_INFO, anything Win32                           |
| Presentation: CLI         | [WhoHolds.Cli](../src/WhoHolds.Cli/)   | Commands, Output format                                | -                                       

### Interop

All definitions inside the [`Interop`](../src/WhoHolds.Core/Interop/) folder must be marked internal.

## Tests

**Tests projects can only reference the project that they're testing and _optionally_ [`WhoHolds.Tests.Shared`](../tests/WhoHolds.Tests.Shared/WhoHolds.Tests.Shared.csproj). Test projects mirror the structure of the project being tested** and may contain additional folders related to testing such as `TestInfrastructure` and additional tests like inside [`Architecture`](../tests/WhoHolds.Core.Tests/Architecture).

## CI Pipeline

The repository uses the [`ci.yml`](../.github/workflows/ci.yml) file which runs a .NET project that uses [ModularPipelines](https://thomhurst.github.io/ModularPipelines/docs/). Extension of the CI pipeline in most cases should be done in this project with one exception so far: disable Defender step.