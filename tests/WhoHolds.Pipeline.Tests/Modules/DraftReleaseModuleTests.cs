using Microsoft.Extensions.Options;
using ModularPipelines.GitHub;
using Octokit;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.Extensions;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class DraftReleaseModuleTests
{
    [Test]
    public void ShouldDependOnSmokeTestModule()
    {
        DraftReleaseModule.ShouldHaveDependsOnAttribute<DraftReleaseModule, SmokeTestModule>();
    }

    [Test]
    [Arguments("not-tag", true)]
    [Arguments("tag", false)]
    public async Task ShouldSkipOnNonTagPush(string refType, bool shouldSkip)
    {
        var pipelineOptions = Options.Create(
            new PipelineSettings { GitHubRefType = refType, GitHubTagRef = "tag" }
        );

        var module = new DraftReleaseModule(pipelineOptions);
        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        var result = await config.SkipCondition.Invoke(null!);
        result.ShouldSkip.ShouldBe(shouldSkip);
    }

    [Test]
    [Arguments(null, "repo")]
    [Arguments("owner", null)]
    public async Task GetOrCreateReleaseDraftAsyncShouldThrowIfOwnerOrRepoIsNull(
        string? owner,
        string? repo
    )
    {
        var repositoryInfo = IGitHubRepositoryInfo.Mock();
        repositoryInfo.Owner.Returns(owner);
        repositoryInfo.RepositoryName.Returns(repo);

        var client = IReleasesClient.Mock();

        await Should.ThrowAsync<ArgumentNullException>(() =>
            DraftReleaseModule.GetOrCreateReleaseDraftAsync(
                repositoryInfo.Object,
                client.Object,
                string.Empty
            )
        );
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
            DraftReleaseModule.GetOrCreateReleaseDraftAsync(repositoryInfo, client, tag)
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

        var existing = await DraftReleaseModule.GetOrCreateReleaseDraftAsync(
            repositoryInfo,
            client,
            tag
        );
        existing.Id.ShouldBe(id);

        client.GetAll(owner: owner, name: repo).WasCalled(Times.Once);
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
        var newRelease = new NewRelease(tag) { Draft = true, GenerateReleaseNotes = true };
        client
            .Create(
                owner,
                repo,
                Is<NewRelease>(r => r!.TagName == tag && r.Draft && r.GenerateReleaseNotes)
            )
            .Returns(release);

        var created = await DraftReleaseModule.GetOrCreateReleaseDraftAsync(
            repositoryInfo,
            client,
            tag
        );
        created.Id.ShouldBe(id);

        client
            .Create(
                owner,
                repo,
                Is<NewRelease>(r => r!.TagName == tag && r.Draft && r.GenerateReleaseNotes)
            )
            .WasCalled(Times.Once);
    }
}
