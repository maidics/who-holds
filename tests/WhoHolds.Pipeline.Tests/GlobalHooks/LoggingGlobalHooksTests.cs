using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using WhoHolds.Pipeline.GlobalHooks;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.GlobalHooks;

public sealed class LoggingGlobalHooksTests
{
    [Test]
    public async Task OnPipelineStartAsyncShouldLog()
    {
        var options = Options.Create(
            new PipelineSettings { GitHubRefName = "ref-name", GitHubRefType = "ref-type" }
        );

        var hooks = new LoggingGlobalHooks(options);

        var logger = new FakeModuleLogger();

        var context = IPipelineHookContext.Mock();
        context.Logger.Returns(logger);

        await hooks.OnPipelineStartAsync(context);

        logger.Collector.Count.ShouldBe(1);
        logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.Collector.LatestRecord.Message.ShouldBe(
            $"Running pipeline for {options.Value.GitHubRefType} ref: '{options.Value.GitHubRefName}'."
        );
    }
}
