using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class CppBuildToolRequirementTests
{
    private static Task<RequirementDecision> EvaluateAsync(
        CppBuildToolLookupResult result,
        string refType = "tag",
        FakeModuleLogger? logger = null
    )
    {
        var pipelineOptions = Options.Create(
            new PipelineSettings { GitHubRefType = refType, GitHubTagRef = "tag" }
        );

        IPipelineHookContextMock context = null!;

        if (logger is not null)
        {
            context = IPipelineHookContext.Mock();
            context.Logger.Returns(logger);
        }

        var cppBuildToolOptions = Options.Create(new CppBuildToolSettings { VsWhere = "test" });

        var resolver = ICppBuildToolLocator.Mock();
        resolver.LocateAsync(Any()).ReturnsAsync(() => Task.FromResult(result));

        return new CppBuildToolRequirement(
            resolver.Object,
            pipelineOptions,
            cppBuildToolOptions
        ).MustAsync(context?.Object!);
    }

    [Test]
    public async Task ShouldReturnEarlyOnNonTagPush()
    {
        var logger = new FakeModuleLogger();

        var result = await EvaluateAsync(
            new CppBuildToolLookupResult(default, default),
            "not-tag",
            logger
        );
        result.Success.ShouldBeTrue();

        logger.Collector.Count.ShouldBe(1);
        logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.Collector.LatestRecord.Message.ShouldBe(
            $"{nameof(CppBuildToolRequirement)} returning early on non tag ref push."
        );
    }

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsNotFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(false, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsFoundButCppToolsAreMissing()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(true, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnSuccessWhenVsWhereAndCppToolsAreFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(true, "test"));
        decision.Success.ShouldBeTrue();
    }
}
