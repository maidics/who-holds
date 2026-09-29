using Microsoft.Extensions.Options;
using ModularPipelines.Context.Domains.Shell;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class SmokeTestModuleTests
{
    [Test]
    public void ShouldDependOnPublishModule()
    {
        SmokeTestModule.ShouldHaveDependsOnAttribute<SmokeTestModule, PublishModule>();
    }

    [Test]
    public async Task ShouldSkipOnNonTagRefPushes()
    {
        var options = Options.Create(
            new PipelineSettings { GitHubRefType = "not-tag", GitHubTagRef = "tag" }
        );

        var module = new SmokeTestModule(options);

        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        var result = await config.SkipCondition(null!);
        result.ShouldSkip.ShouldBeTrue();
    }

    [Test]
    public void EnsurePublishedShouldThrowIfPublishedBuildIsNull()
    {
        var result = ModuleResult.CreateSuccess<PublishedBuild>(null);

        var ex = Should.Throw<ArgumentNullException>(() => SmokeTestModule.EnsurePublished(result));
        ex.Message.ShouldContain("publishedBuild");
    }

    [Test]
    public void EnsurePublishedShouldThrowIfPublishedFileDoesNotExist()
    {
        var publishedBuild = new PublishedBuild("wh.exe", AppContext.BaseDirectory, "1.0.0");

        var result = ModuleResult.CreateSuccess(publishedBuild);

        var ex = Should.Throw<FileNotFoundException>(() => SmokeTestModule.EnsurePublished(result));
        ex.Message.ShouldBe($"Published file not found at path: '{publishedBuild.FilePath}'.");
    }

    [Test]
    public async Task ExecuteVersionCommandAsyncShouldExecuteVersionCommandOnPublishedFilePath()
    {
        var publishedBuild = new PublishedBuild("wh.exe", AppContext.BaseDirectory, "1.0.0");
        var context = ICommandContext.Mock();

        await SmokeTestModule.ExecuteVersionCommandAsync(
            publishedBuild,
            context,
            CancellationToken.None
        );

        context
            .ExecuteCommandLineTool(
                o =>
                    o.Tool == publishedBuild.FilePath
                    && o.Arguments is not null
                    && o.Arguments.Count() == 1
                    && o.Arguments.First() == "--version",
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }

    [Test]
    [Arguments("1.0.0", "1.0.0")] // stdout contains no '+'
    [Arguments("1.0.0+sha", "2.0.0")]
    public void ThrowIfVersionOutputInvalidShouldThrowIfVersionOutputIsInvalid(
        string stdout,
        string expectedVersion
    )
    {
        var ex = Should.Throw<InvalidOperationException>(() =>
            SmokeTestModule.ThrowIfVersionOutputInvalid(stdout, expectedVersion)
        );
        ex.Message.ShouldBe(
            $"Unexpected version output: '{stdout.Trim()}'. Expected format: '{{version}}+{{git commit sha}}'."
        );
    }

    [Test]
    [Arguments("1.0.0+sha", "1.0.0")]
    [Arguments("2.0.0+sha", "2.0.0")]
    public void ThrowIfVersionOutputInvalidShouldNotThrowIfVersionIsValid(
        string stdout,
        string expectedVersion
    )
    {
        Should.NotThrow(() => SmokeTestModule.ThrowIfVersionOutputInvalid(stdout, expectedVersion));
    }
}
