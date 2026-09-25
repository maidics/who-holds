using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class VersionTagFormatRequirementTests
{
    [Test]
    public async Task ShouldReturnEarlyOnNonTagPush()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefTypeEnvVar] = "not-tag" }
            )
            .Build();

        var requirement = new VersionTagFormatRequirement(config);

        var logger = new FakeModuleLogger();

        var context = IPipelineHookContext.Mock();
        context.Logger.Returns(logger);

        var result = await requirement.MustAsync(context);
        result.Success.ShouldBeTrue();

        logger.Collector.Count.ShouldBe(1);
        logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.Collector.LatestRecord.Message.ShouldBe("Returning early on non tag push ref.");
    }

    [Test]
    [Arguments("test")]
    [Arguments("1-1-1")]
    [Arguments("1_1_1")]
    [Arguments("1.1.1.1")]
    [Arguments("v1.1.1.1")]
    public async Task ShouldReturnFailedWhenTagIsNotValid(string tag)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefTypeEnvVar] = Repo.GitHubTagRef,
                    [Repo.GitHubRefNameEnvVar] = tag,
                }
            )
            .Build();

        var requirement = new VersionTagFormatRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeFalse();
        result.Reason.ShouldNotBeNull();
        result.Reason.ShouldContain($"Invalid tag: '{tag}'");
    }

    [Test]
    [Arguments("v0.0.0")]
    [Arguments("v1.2.0")]
    public async Task ShouldPassIfTagIsValid(string tag)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefTypeEnvVar] = Repo.GitHubTagRef,
                    [Repo.GitHubRefNameEnvVar] = tag,
                }
            )
            .Build();

        var requirement = new VersionTagFormatRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeTrue();
        result.Reason.ShouldBeNull();
    }
}
