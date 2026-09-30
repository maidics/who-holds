using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Modules;
using Octokit;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<SmokeTestModule>]
public sealed class DraftReleaseModule : Module // TODO: add to pipeline, do tests
{
    private readonly PipelineSettings _pipelineSettings;

    public DraftReleaseModule(IOptions<PipelineSettings> pipelineOptions)
    {
        _pipelineSettings = pipelineOptions.Value;
    }

    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration.Create().WithSkipWhen(_ => !_pipelineSettings.IsTagPush).Build();
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var result = (await context.GetModule<PublishModule>()).EnsurePublished();

        var tag = _pipelineSettings.GitHubRefName;

        throw new NotImplementedException();
    }
}
