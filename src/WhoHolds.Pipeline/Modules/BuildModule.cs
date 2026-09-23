using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<RestoreModule>]
public sealed class BuildModule(string? releaseVersion) : Module
{
    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetBuildOptions
        {
            NoRestore = true,
            Nologo = true,
            ProjectSolution = Repo.Solution,
            Configuration = Repo.Configuration,
            Properties = releaseVersion is null
                ? null
                : [new KeyValue(Repo.DotNetVersionArgumentKey, releaseVersion)],
        };

        await context.DotNet().Build(options, cancellationToken: cancellationToken);
    }
}
