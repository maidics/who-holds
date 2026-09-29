using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

public sealed class RestoreModule : Module // TODO: document glue module adr: public since only tests reference this project
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
        await RestoreAsync(context.DotNet(), _settings.Solution, cancellationToken);
    }

    public static async Task RestoreAsync(
        IDotNet dotnet,
        string solution,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetRestoreOptions { ProjectSolution = solution };

        await dotnet.Restore(options, cancellationToken: cancellationToken);
    }
}
