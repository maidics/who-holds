using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Modules;

public sealed class RestoreModule : Module
{
    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetRestoreOptions { ProjectSolution = Repo.Solution };

        await context.DotNet().Restore(options, cancellationToken: cancellationToken);
    }
}
