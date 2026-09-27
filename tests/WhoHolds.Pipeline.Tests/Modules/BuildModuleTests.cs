using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class BuildModuleTests
{
    [Test]
    public void ShouldDependOnRestoreModule()
    {
        PipelineTesting.ShouldHaveDependsOnAttribute<BuildModule, RestoreModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        var testing = new PipelineTesting(_ => new RestoreModule(), _ => new BuildModule());

        var summary = await testing.BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetBuildOptions
        {
            Nologo = true,
            NoRestore = true,
            ProjectSolution = Repo.Solution,
            Configuration = Repo.Configuration,
        };

        testing.Dotnet.Build(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new PipelineTesting(_ => new BuildModule());

        testing
            .Dotnet.Build(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Build failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(BuildModule));
    }
}
