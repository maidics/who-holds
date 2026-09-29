using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
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
        await BuildAsync(
            _settings.Solution,
            _settings.Configuration,
            context.DotNet(),
            cancellationToken
        );
    }

    public static async Task BuildAsync(
        string solution,
        string configuration,
        IDotNet dotnet,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetBuildOptions
        {
            NoRestore = true,
            Nologo = true,
            ProjectSolution = solution,
            Configuration = configuration,
        };

        await dotnet.Build(options, cancellationToken: cancellationToken);
    }
}
