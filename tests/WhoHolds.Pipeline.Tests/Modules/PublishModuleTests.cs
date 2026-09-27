using Microsoft.Extensions.Options;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.Extensions;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class PublishModuleTests
{
    private static IOptions<PipelineSettings> CreatePipelineOptions(
        string refType,
        string refName
    ) => Options.Create(new PipelineSettings { GitHubRefType = refType, GitHubRefName = refName });

    private static readonly IOptions<PublishSettings> _publishOptions = Options.Create(
        new PublishSettings
        {
            OutputDirectory = "output-dir",
            Runtime = "runtime",
            ProjectPath = "project",
        });

    [Test]
    public void ShouldSkipOnNonTagPush()
    {
        var pipelineOptions = CreatePipelineOptions(string.Empty, string.Empty);
        var module = new PublishModule(pipelineOptions, null!, null!);
        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        // TODO
    }

    //TODO
    // [Test]
    // public async Task ShouldRunModule()
    // {
    //     const string version = "version";
    //
    //     var pipelineOptions = CreatePipelineOptions("tag", version);
    //
    //     var resolver = new FakeReleaseVersionResolver();
    //
    //     var testing = new PipelineTesting(
    //         _ => new RestoreModule(null!),
    //         _ => new BuildModule(null!),
    //         _ => new TestModule(null!),
    //         _ => new PublishModule(pipelineOptions, _publishOptions, resolver)
    //     );
    //
    //     var summary = await testing.BuildAndRunAsync();
    //     summary.Status.ShouldBe(Status.Successful);
    // }
    
    // TODO: add failure path
}
