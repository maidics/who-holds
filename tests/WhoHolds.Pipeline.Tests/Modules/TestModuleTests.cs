using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class TestModuleTests : DotNetModuleTestBase<TestModule>
{
    [Test]
    public void ShouldDependOnBuildModule()
    {
        ShouldHaveDependsOnAttribute<BuildModule>();
    }

    [Test]
    public override async Task ShouldRunModule()
    {
        var summary = await BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = Repo.Configuration,
            Solution = Repo.Solution,
        };

        _dotNet.Test(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public override async Task ShouldFailPipelineWhenModuleFails()
    {
        _dotNet
            .Test(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Running tests failed."));

        var ex = await Should.ThrowAsync<Exception>(BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(TestModule));
    }
}
