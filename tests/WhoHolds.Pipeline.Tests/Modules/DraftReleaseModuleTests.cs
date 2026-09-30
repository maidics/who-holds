using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.GitHub;
using ModularPipelines.Logging;
using Octokit;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.Extensions;
using WhoHolds.Pipeline.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class DraftReleaseModuleTests
{
    [Test]
    public void ShouldDependOnSmokeTestModule()
    {
        DraftReleaseModule.ShouldHaveDependsOnAttribute<DraftReleaseModule, SmokeTestModule>();
    }

    [Test]
    [Arguments("not-tag")]
    [Arguments("tag")]
    public async Task ShouldSkipOnNonTagPush(string refType)
    {
        var pipelineOptions = Options.Create(
            new PipelineSettings { GitHubRefType = refType, GitHubTagRef = "tag" }
        );

        var module = new DraftReleaseModule(pipelineOptions);
        var config = module.GetConfiguration();
        await config.ShouldSkipOnTagPushAsync(pipelineOptions.Value.IsTagPush);
    }

    [Test]
    [Arguments(null, "repo")]
    [Arguments("owner", null)]
    public void GetOwnerAndRepoShouldThrowIfOwnerAndRepoIsNull(string? owner, string? repo)
    {
        var repositoryInfo = IGitHubRepositoryInfo.Mock();
        repositoryInfo.Owner.Returns(owner);
        repositoryInfo.RepositoryName.Returns(repo);

        var ex = Should.Throw<ArgumentNullException>(() =>
            DraftReleaseModule.GetOwnerAndRepo(repositoryInfo)
        );
        ex.Message.ShouldContain(owner is null ? nameof(owner) : nameof(repo));
    }

    [Test]
    public async Task GetOrCreateReleaseDraftAsyncShouldThrowIfReleaseExistsAndIsNotDraft()
    {
        const string owner = nameof(owner);
        const string repo = nameof(repo);
        var repositoryInfo = IGitHubRepositoryInfo.Mock();
        repositoryInfo.Owner.Returns(owner);
        repositoryInfo.RepositoryName.Returns(repo);

        const string tag = "v1.0.0";

        var release = Release.Create(tagName: tag, draft: false);

        var client = IReleasesClient.Mock();
        client.GetAll(owner: owner, name: repo).Returns([release]);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            DraftReleaseModule.GetOrCreateReleaseDraftAsync(
                owner,
                repo,
                client,
                tag,
                IModuleLogger.Mock().Object
            )
        );
        ex.Message.ShouldBe($"Release {tag} is already published.");

        client.GetAll(owner: owner, name: repo).WasCalled(Times.Once);
    }

    [Test]
    public async Task GetOrCreateReleaseDraftAsyncShouldReturnExistingRelease()
    {
        const string owner = nameof(owner);
        const string repo = nameof(repo);
        var repositoryInfo = IGitHubRepositoryInfo.Mock();
        repositoryInfo.Owner.Returns(owner);
        repositoryInfo.RepositoryName.Returns(repo);

        const string tag = "v1.0.0";
        const long id = 120;

        var release = Release.Create(tagName: tag, draft: true, id: id);

        var client = IReleasesClient.Mock();
        client.GetAll(owner: owner, name: repo).Returns([release]);

        var logger = new FakeModuleLogger();

        var existing = await DraftReleaseModule.GetOrCreateReleaseDraftAsync(
            owner,
            repo,
            client,
            tag,
            logger
        );
        existing.Id.ShouldBe(id);

        client.GetAll(owner: owner, name: repo).WasCalled(Times.Once);

        logger.Collector.Count.ShouldBe(1);
        logger.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.LatestRecord.Message.ShouldBe($"Found existing release: {tag} ({release.Id})");
    }

    [Test]
    public async Task GetOrCreateReleaseDraftAsyncShouldCreateReleaseIfItDoesNotExist()
    {
        const string owner = nameof(owner);
        const string repo = nameof(repo);
        var repositoryInfo = IGitHubRepositoryInfo.Mock();
        repositoryInfo.Owner.Returns(owner);
        repositoryInfo.RepositoryName.Returns(repo);

        const string tag = "v1.0.0";
        const long id = 120;

        var release = Release.Create(tagName: tag, draft: false, id: id);

        var client = IReleasesClient.Mock();
        client
            .Create(
                owner,
                repo,
                Is<NewRelease>(r => r!.TagName == tag && r.Draft && r.GenerateReleaseNotes)
            )
            .Returns(release);

        var logger = new FakeModuleLogger();

        var created = await DraftReleaseModule.GetOrCreateReleaseDraftAsync(
            owner,
            repo,
            client,
            tag,
            logger
        );
        created.Id.ShouldBe(id);

        client
            .Create(
                owner,
                repo,
                Is<NewRelease>(r => r!.TagName == tag && r.Draft && r.GenerateReleaseNotes)
            )
            .WasCalled(Times.Once);

        logger.Collector.Count.ShouldBe(1);
        logger.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.LatestRecord.Message.ShouldBe($"Creating new release for {tag} tag.");
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(3)]
    public async Task UploadAssetAsyncShouldDeleteFilesWhenReleaseHasAssets(int assetCount)
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        var assets = Enumerable
            .Range(0, assetCount)
            .Select(_ => ReleaseAsset.Create(name: fs.CreateTestFile()))
            .ToList();

        var release = Release.Create(assets: assets);

        var logger = new FakeModuleLogger();

        const string owner = nameof(owner);
        const string repo = nameof(repo);

        var client = IReleasesClient.Mock();

        await DraftReleaseModule.UploadAssetAsync(
            release,
            logger,
            fs.CreateTestFile(),
            owner,
            repo,
            client,
            CancellationToken.None
        );

        var logs = logger.Collector.GetSnapshot();

        if (assetCount > 0)
        {
            logs.Count.ShouldBe(2);

            logs.FirstOrDefault(r =>
                    r.Level == LogLevel.Information
                    && r.Message
                        == $"Deleted existing asset(s) on {release.TagName} ({release.Id}) release: {string.Join(", ", assets.Select(a => a.Name))}"
                )
                .ShouldNotBeNull();

            client
                .DeleteAsset(Any(), Any(), Any())
                .WasCalled(assetCount == 1 ? Times.Once : Times.AtLeastOnce);
        }
        else
        {
            logs.Count.ShouldBe(1);
        }
    }

    [Test]
    public async Task UploadAssetAsyncShouldUploadAsset()
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        var release = Release.Create();
        var logger = new FakeModuleLogger();
        var file = fs.CreateTestFile();
        const string owner = nameof(owner);
        const string repo = nameof(repo);
        var client = IReleasesClient.Mock();

        await DraftReleaseModule.UploadAssetAsync(
            release,
            logger,
            file,
            owner,
            repo,
            client,
            CancellationToken.None
        );

        var fileName = Path.GetFileName(file);

        logger.Collector.Count.ShouldBe(1);
        logger.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.LatestRecord.Message.ShouldBe(
            $"Uploading {fileName} to {release.TagName} ({release.Id})..."
        );

        client
            .UploadAsset(r => r.Id == release.Id, u => u.FileName == fileName, Any())
            .WasCalled(Times.Once);
    }
}
