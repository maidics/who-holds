using Microsoft.Extensions.Options;
using ModularPipelines.Context.Domains.Shell;
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
    [Arguments("tag", false)]
    [Arguments("not-tag", true)]
    public async Task ShouldSkipOnNonTagRefPushes(string refName, bool shouldSkip)
    {
        var options = Options.Create(
            new PipelineSettings { GitHubRefType = refName, GitHubTagRef = "tag" }
        );

        var module = new SmokeTestModule(options);

        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        var result = await config.SkipCondition(null!);
        result.ShouldSkip.ShouldBe(shouldSkip);
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
