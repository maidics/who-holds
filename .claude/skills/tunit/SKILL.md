---
name: tunit
description: Write or troubleshoot tests using TUnit, or TUnit.Mocks, or migrate tests to TUnit. Routes to task-specific official documentation. Use only for tasks involving these packages or migration to TUnit.
---

# TUnit

Use this skill to find the TUnit guidance needed for the current task.  The documentation contains the API details and examples; do not load the whole site.

**Important**: referencing a test project from another test projects results in duplicated test runs. For shared concerns use the [WhoHolds.Tests.Shared](../../../tests/WhoHolds.Tests.Shared) project.

## Load only what the task needs

- For an unlisted topic or a broken route, fetch [the documentation index](https://tunit.dev/llms.txt), search it for the feature or diagnostic, then fetch only the matching page. Do not fetch `llms-full.txt` or crawl every link. 
- Follow related links only to resolve a specific gap. Prefer the `.md` version: remove a trailing slash and append `.md` to an HTML documentation path, before any fragment. Keep existing `.md` paths unchanged. Resolve relative links against the fetched page's URL.
- Cite the documentation used. If current documentation differs from the installed API, check version-matched source or local API evidence before proposing an upgrade. If fetching fails, use available local documentation and state what could not be verified; do not guess APIs.

## Topic routes

Choose the link that matches the task; these are alternatives, not a reading checklist.

| Task | Documentation                                                                                                                                                                                                     |
| --- |-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Create or configure a project | [Installation](https://tunit.dev/docs/getting-started/installation.md)                                                                                                                                            |
| Write a basic test | [First test](https://tunit.dev/docs/getting-started/writing-your-first-test.md)                                                                                                                                   |
| Assertions, exceptions, or assertion composition | Assertions from TUnit are not used. Use [Shouldly](https://docs.shouldly.org/documentation/) package instead.                                                                                                     |
| Parameterized tests | [Arguments](https://tunit.dev/docs/writing-tests/arguments.md), [method data](https://tunit.dev/docs/writing-tests/method-data-source.md), or [matrix data](https://tunit.dev/docs/writing-tests/matrix-tests.md) |
| Setup, teardown, or hooks | [Lifecycle](https://tunit.dev/docs/writing-tests/lifecycle.md)                                                                                                                                                    |
| Fixtures, shared resources, or dependency injection | [Class data sources](https://tunit.dev/docs/writing-tests/class-data-source.md) or [dependency injection](https://tunit.dev/docs/writing-tests/dependency-injection.md)                                           |
| Concurrency or dependencies between tests | [Parallelism](https://tunit.dev/docs/execution/parallelism.md) or [ordering](https://tunit.dev/docs/writing-tests/ordering.md)                                                                                    |
| Run tests or select tests | [Running tests](https://tunit.dev/docs/getting-started/running-your-tests.md) or [filters](https://tunit.dev/docs/execution/test-filters.md)                                                                      |
| Migrate an existing suite | [xUnit](https://tunit.dev/docs/migration/xunit.md), [NUnit](https://tunit.dev/docs/migration/nunit.md), or [MSTest](https://tunit.dev/docs/migration/mstest.md), matching the source framework                    |
| Mocking | [TUnit.Mocks](https://tunit.dev/docs/writing-tests/mocking.md)                                                                                                                                                    |
| Aspire integration testing | [Aspire](https://tunit.dev/docs/examples/aspire.md)                                                                                                                                                               |
| ASP.NET Core integration testing | [ASP.NET Core](https://tunit.dev/docs/examples/aspnet.md)                                                                                                                                                         |
| Playwright browser testing | [Playwright](https://tunit.dev/docs/examples/playwright.md)                                                                                                                                                       |
| Testcontainers and container fixtures | [ASP.NET Core integration examples](https://tunit.dev/docs/examples/aspnet.md) (see "With Testcontainers")                                                                                                        |
| Native AOT, trimming, or reflection mode | [AOT compatibility](https://tunit.dev/docs/writing-tests/aot.md) or [engine modes](https://tunit.dev/docs/execution/engine-modes.md)                                                                              |
| Custom data sources | [Data source generators](https://tunit.dev/docs/extending/data-source-generators.md)                                                                                                                              |
| Event receivers | [Event subscribing](https://tunit.dev/docs/writing-tests/event-subscribing.md)                                                                                                                                    |
| TestContext and test metadata | [Test context](https://tunit.dev/docs/writing-tests/test-context.md)                                                                                                                                              |
| Diagnostics, output, or tracing | [Logging](https://tunit.dev/docs/extending/logging.md), [test artifacts](https://tunit.dev/docs/writing-tests/artifacts.md), or [OpenTelemetry](https://tunit.dev/docs/examples/opentelemetry.md)                 |
| Cancellation | [Cancelling a test](https://tunit.dev/docs/execution/cancellation.md)                                                                                                                                             |
| Discovery, build, or execution failures | [Troubleshooting](https://tunit.dev/docs/troubleshooting.md)                                                                                                                                                      |

For other topics, such as analyzers or framework-specific integrations, use the index selectively.

## Essential distinctions

- Tests run in parallel by default. Account for shared mutable state and resource lifetimes.
- TUnit uses Microsoft.Testing.Platform and `--treenode-filter`. Match command syntax to the project's SDK and runner mode; do not substitute VSTest `--filter` or assume every `dotnet test` invocation needs a `--` separator.
- Source-generated discovery is the default; reflection mode also exists. Check the relevant documentation when changing discovery or Native AOT behavior.