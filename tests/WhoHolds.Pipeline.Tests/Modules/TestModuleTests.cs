using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
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
        TestModule.ShouldHaveDependsOnAttribute<TestModule, BuildModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        var dotnet = IDotNet.Mock();
        var context = IModuleContext.CreateWithDotNetMock(dotnet);
        var module = new TestModule(_options);
        await module.TestExecuteAsync(context);

        var expectedOptions = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = _options.Value.Configuration,
            Solution = _options.Value.Solution,
        };

        dotnet.Test(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }
}
