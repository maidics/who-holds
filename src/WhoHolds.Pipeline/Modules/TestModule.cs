using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<BuildModule>]
public sealed class TestModule : Module
{
    private readonly PipelineSettings _settings;

    public TestModule(IOptions<PipelineSettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = _settings.Configuration,
            Solution = _settings.Solution,
        };

        await context.DotNet().Test(options, cancellationToken: cancellationToken);
    }
}
