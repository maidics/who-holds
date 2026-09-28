using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.Extensions;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

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
        }
    );

    [Test]
    [Arguments("not-tag", true)]
    [Arguments("tag", false)]
    public async Task ShouldSkipOnNonTagPush(string refType, bool shouldSkip)
    {
        var pipelineOptions = CreatePipelineOptions(refType, string.Empty);
        var module = new PublishModule(pipelineOptions, _publishOptions, null!);
        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        var result = await config.SkipCondition.Invoke(null!);
        result.ShouldSkip.ShouldBe(shouldSkip);
    }

    [Test]
    public async Task ShouldRunModule()
    {
        const string version = "v1.0.0";
        var pipelineOptions = CreatePipelineOptions("tag", version);
        var versionResolver = new FakeReleaseVersionResolver(version);
        var module = new PublishModule(pipelineOptions, _publishOptions, versionResolver);

        var dotnet = IDotNet.Mock();
        var context = IModuleContext.CreateWithDotNetMock(dotnet);
        await module.TestExecuteAsync(context);

        dotnet
            .Publish(
                o =>
                    o.NoRestore == false
                    && o.NoBuild == false
                    && o.Nologo == true
                    && o.ProjectSolution == _publishOptions.Value.ProjectPath
                    && o.Configuration == pipelineOptions.Value.Configuration
                    && o.Runtime == _publishOptions.Value.Runtime
                    && o.Output == _publishOptions.Value.OutputDirectory
                    && o.Properties is not null
                    && o.Properties.Count() == 1
                    && o.Properties.FirstOrDefault(p => p.Key == "Version" && p.Value == version)
                        is not null,
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }
}
