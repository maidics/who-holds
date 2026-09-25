using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using ModularPipelines.Interfaces;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.GlobalHooks;

public sealed class LoggingGlobalHooks(IConfiguration configuration) : IPipelineGlobalHooks
{
    public Task OnPipelineStartAsync(IPipelineHookContext context)
    {
        var refType = configuration.GetValue<string>(Repo.GitHubRefTypeEnvVar);
        var refName = configuration.GetValue<string>(Repo.GitHubRefNameEnvVar);

        context.Logger.LogInformation(
            "Running pipeline for {RefType} ref: '{RefName}'.",
            refType,
            refName
        );

        return Task.CompletedTask;
    }
}
