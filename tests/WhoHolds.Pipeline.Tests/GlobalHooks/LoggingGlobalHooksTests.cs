using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.GlobalHooks;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.GlobalHooks;

public sealed class LoggingGlobalHooksTests
{
    [Test]
    [Arguments(null, null)]
    [Arguments("", "")]
    [Arguments("tag", "v1.0.0")]
    public async Task OnPipelineStartAsyncShouldLog(string? refType, string? refName)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefTypeEnvVar] = refType,
                    [Repo.GitHubRefNameEnvVar] = refName,
                }
            )
            .Build();

        var hooks = new LoggingGlobalHooks(config);

        var logger = new FakeModuleLogger();

        var context = IPipelineHookContext.Mock();
        context.Logger.Returns(logger);

        await hooks.OnPipelineStartAsync(context);

        logger.Collector.Count.ShouldBe(1);
        logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.Collector.LatestRecord.Message.ShouldBe(
            $"Running pipeline for {refType ?? "(null)"} ref: '{refName ?? "(null)"}'."
        );
    }
}
