using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class CppBuildToolRequirementTests
{
    private static Task<RequirementDecision> EvaluateAsync(
        CppBuildToolLookupResult result,
        string tag = Repo.GitHubTagRef,
        FakeModuleLogger? logger = null
    )
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefTypeEnvVar] = tag }
            )
            .Build();

        IPipelineHookContextMock context = null!;

        if (logger is not null)
        {
            context = IPipelineHookContext.Mock();
            context.Logger.Returns(logger);
        }

        return new CppBuildToolRequirement(new TestCppBuildToolLocator(result), config).MustAsync(
            context?.Object!
        );
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
        logger.Collector.LatestRecord.Message.ShouldBe("Returning early on non tag push ref.");
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
