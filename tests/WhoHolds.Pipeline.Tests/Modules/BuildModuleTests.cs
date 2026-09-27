using Microsoft.Extensions.Options;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class BuildModuleTests
{
    private static readonly IOptions<PipelineSettings> _options = Options.Create(
        new PipelineSettings { Configuration = "test" }
    );

    [Test]
    public void ShouldDependOnRestoreModule()
    {
        PipelineTesting.ShouldHaveDependsOnAttribute<BuildModule, RestoreModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        var testing = new PipelineTesting(
            _ => new RestoreModule(_options),
            _ => new BuildModule(_options)
        );

        var summary = await testing.BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetBuildOptions
        {
            Nologo = true,
            NoRestore = true,
            ProjectSolution = _options.Value.Solution,
            Configuration = _options.Value.Configuration,
        };

        testing.Dotnet.Build(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new PipelineTesting(_ => new BuildModule(_options));

        testing
            .Dotnet.Build(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Build failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(BuildModule));
    }
}
