using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<RestoreModule>]
public sealed class BuildModule : Module
{
    private readonly PipelineSettings _settings;

    public BuildModule(IOptions<PipelineSettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetBuildOptions
        {
            NoRestore = true,
            Nologo = true,
            ProjectSolution = _settings.Solution,
            Configuration = _settings.Configuration,
        };

        await context.DotNet().Build(options, cancellationToken: cancellationToken);
    }
}
