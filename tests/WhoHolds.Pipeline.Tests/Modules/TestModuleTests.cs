using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class TestModuleTests
{
    [Test]
    public void ShouldDependOnBuildModule()
    {
        ModuleTesting<TestModule>.ShouldDependOn<BuildModule>();
    }

    [Test]
    public async Task ShouldRunModule()
    {
        var testing = new ModuleTesting<TestModule>();

        var summary = await testing.GetSummaryAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetTestOptions
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = Repo.Configuration,
            Solution = Repo.Solution,
        };

        testing.DotNet.Test(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new ModuleTesting<TestModule>();

        testing
            .DotNet.Test(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Running tests failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.GetSummaryAsync);
        ex.Message.ShouldContain(nameof(TestModule));
    }
}
