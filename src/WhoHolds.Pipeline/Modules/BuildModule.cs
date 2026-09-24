using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<RestoreModule>]
public sealed class BuildModule : Module
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
            Configuration = Repo.Configuration, // no release version because this is the test build, not native aot
        };

        await context.DotNet().Build(options, cancellationToken: cancellationToken);
    }
}
