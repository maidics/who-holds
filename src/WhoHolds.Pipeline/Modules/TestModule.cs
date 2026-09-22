using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<BuildModule>]
public sealed class TestModule : Module
{
    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = Repo.Configuration,
            Solution = Repo.Solution,
        };

        await context.DotNet().Test(options, cancellationToken: cancellationToken);
    }
}
