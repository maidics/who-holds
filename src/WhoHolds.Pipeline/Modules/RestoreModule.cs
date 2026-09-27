using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

public sealed class RestoreModule : Module
{
    private readonly PipelineSettings _settings;

    public RestoreModule(IOptions<PipelineSettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetRestoreOptions { ProjectSolution = _settings.Solution };

        await context.DotNet().Restore(options, cancellationToken: cancellationToken);
    }
}
