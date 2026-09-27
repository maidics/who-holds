using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Interfaces;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.GlobalHooks;

public sealed class LoggingGlobalHooks(IOptions<PipelineSettings> options) : IPipelineGlobalHooks
{
    public Task OnPipelineStartAsync(IPipelineHookContext context)
    {
        context.Logger.LogInformation(
            "Running pipeline for {RefType} ref: '{RefName}'.",
            options.Value.GitHubRefType,
            options.Value.GitHubRefName
        );

        return Task.CompletedTask;
    }
}
