using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class BuildModuleTests : DotNetModuleTestBase<BuildModule>
{
    [Test]
    public void ShouldDependOnRestoreModule()
    {
        ShouldHaveDependsOnAttribute<RestoreModule>();
    }

    [Test]
    public override async Task ShouldRunModule()
    {
        var summary = await BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetBuildOptions
        {
            Nologo = true,
            NoRestore = true,
            ProjectSolution = Repo.Solution,
            Configuration = Repo.Configuration,
        };

        _dotNet.Build(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public override async Task ShouldFailPipelineWhenModuleFails()
    {
        _dotNet.Build(Any(), Any(), Any()).Throws(new InvalidOperationException("Build failed."));

        var ex = await Should.ThrowAsync<Exception>(BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(BuildModule));
    }
}
