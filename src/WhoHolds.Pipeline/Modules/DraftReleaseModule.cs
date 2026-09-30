using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.GitHub;
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

        // get or create release

        // upload assets
    }

    public static async Task<Release> GetOrCreateReleaseDraftAsync(
        IGitHubRepositoryInfo repositoryInfo,
        IReleasesClient releasesClient,
        string tag
    )
    {
        var owner = repositoryInfo.Owner;
        ArgumentNullException.ThrowIfNull(owner);
        var repo = repositoryInfo.RepositoryName;
        ArgumentNullException.ThrowIfNull(repo);

        var existing = (await releasesClient.GetAll(owner, repo)).FirstOrDefault(r =>
            r.TagName == tag
        );

        if (existing is { Draft: false })
            throw new InvalidOperationException($"Release {tag} is already published.");

        return existing
            ?? await releasesClient.Create(
                owner,
                repo,
                new NewRelease(tag) { Draft = true, GenerateReleaseNotes = true }
            );
    }
}
