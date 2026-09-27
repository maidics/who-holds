using Microsoft.Extensions.Options;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class TestModuleTests
{
    private static readonly IOptions<PipelineSettings> _options = Options.Create(
        new PipelineSettings { Configuration = "test" }
    );

    [Test]
    public void ShouldDependOnBuildModule()
    {
        PipelineTesting.ShouldHaveDependsOnAttribute<TestModule, BuildModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        var testing = new PipelineTesting(
            _ => new RestoreModule(_options),
            _ => new BuildModule(_options),
            _ => new TestModule(_options)
        );

        var summary = await testing.BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = _options.Value.Configuration,
            Solution = _options.Value.Solution,
        };

        testing.Dotnet.Test(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new PipelineTesting(_ => new TestModule(_options));

        testing
            .Dotnet.Test(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Running tests failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(TestModule));
    }
}
