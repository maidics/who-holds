using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.GitHub;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Logging;
using ModularPipelines.Modules;
using Octokit;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<SmokeTestModule>]
public sealed class DraftReleaseModule : Module
{
    private readonly PipelineSettings _pipelineSettings;

    public DraftReleaseModule(IOptions<PipelineSettings> pipelineOptions)
    {
        _pipelineSettings = pipelineOptions.Value;
    }

    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration.Create().WithTagPushSkip(_pipelineSettings.IsTagPush).Build();
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var result = (await context.GetModule<PublishModule>()).EnsurePublished();

        var (owner, repo) = GetOwnerAndRepo(context.GitHub().RepositoryInfo);

        var releaseClient = context.GitHub().Client.Repository.Release;

        var release = await GetOrCreateReleaseDraftAsync(
            owner,
            repo,
            releaseClient,
            _pipelineSettings.GitHubRefName,
            context.Logger
        );

        await UploadAssetAsync(
            release,
            context.Logger,
            result.FilePath,
            owner,
            repo,
            releaseClient,
            cancellationToken
        );
    }

    public static (string owner, string repo) GetOwnerAndRepo(IGitHubRepositoryInfo repositoryInfo)
    {
        var owner = repositoryInfo.Owner;
        ArgumentNullException.ThrowIfNull(owner);
        var repo = repositoryInfo.RepositoryName;
        ArgumentNullException.ThrowIfNull(repo);

        return (owner, repo);
    }

    public static async Task<Release> GetOrCreateReleaseDraftAsync(
        string owner,
        string repo,
        IReleasesClient releasesClient,
        string tag,
        IModuleLogger logger
    )
    {
        var existing = (await releasesClient.GetAll(owner, repo)).FirstOrDefault(r =>
            r.TagName == tag
        );

        if (existing is { Draft: false })
            throw new InvalidOperationException($"Release {tag} is already published.");

        if (existing is not null)
        {
            logger.LogInformation("Found existing release: {Tag} ({ReleaseId})", tag, existing.Id);
            return existing;
        }

        logger.LogInformation("Creating new release for {Tag} tag.", tag);

        return await releasesClient.Create(
            owner,
            repo,
            new NewRelease(tag) { Draft = true, GenerateReleaseNotes = true }
        );
    }

    public static async Task UploadAssetAsync(
        Release release,
        IModuleLogger logger,
        string filePath,
        string owner,
        string repo,
        IReleasesClient releasesClient,
        CancellationToken cancellationToken
    )
    {
        var assets = release.Assets;

        if (assets.Count > 0)
        {
            foreach (var asset in release.Assets)
            {
                await releasesClient.DeleteAsset(owner, repo, asset.Id);
            }

            logger.LogInformation(
                "Deleted existing asset(s) on {Tag} ({ReleaseId}) release: {AssetNames}",
                release.TagName,
                release.Id,
                assets.Select(a => a.Name)
            );
        }

        await using var stream = File.OpenRead(filePath);

        var fileName = Path.GetFileName(filePath);

        logger.LogInformation(
            "Uploading {FileName} to {Tag} ({ReleaseId})...",
            fileName,
            release.TagName,
            release.Id
        );

        await releasesClient.UploadAsset(
            release,
            new ReleaseAssetUpload(
                fileName,
                "application/octet-stream",
                stream,
                TimeSpan.FromMinutes(5)
            ),
            cancellationToken
        );
    }
}
