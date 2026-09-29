using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class TestModuleTests
{
    [Test]
    public void ShouldDependOnBuildModule()
    {
        TestModule.ShouldHaveDependsOnAttribute<TestModule, BuildModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        const string configuration = nameof(configuration);
        const string solution = nameof(solution);
        var dotnet = IDotNet.Mock();
        await TestModule.TestAsync(configuration, solution, dotnet, CancellationToken.None);

        var expectedOptions = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
            Solution = solution,
        };

        dotnet.Test(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }
}
