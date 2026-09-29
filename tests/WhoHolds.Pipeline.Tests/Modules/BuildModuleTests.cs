using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class BuildModuleTests
{
    [Test]
    public void ShouldDependOnRestoreModule()
    {
        BuildModule.ShouldHaveDependsOnAttribute<BuildModule, RestoreModule>();
    }

    [Test]
    public async Task BuildAsyncShouldBuild()
    {
        const string solution = nameof(solution);
        const string configuration = nameof(configuration);
        var dotnet = IDotNet.Mock();

        await BuildModule.BuildAsync(solution, configuration, dotnet, CancellationToken.None);

        var expectedOptions = new DotNetBuildOptions
        {
            Nologo = true,
            NoRestore = true,
            ProjectSolution = solution,
            Configuration = configuration,
        };

        dotnet.Build(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }
}
