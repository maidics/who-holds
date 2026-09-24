using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests
{
    [Test]
    public void ShouldNotDependOnAnyModule()
    {
        ModuleTesting<RestoreModule>.ShouldNotDependOnAnyModule();
    }

    [Test]
    public async Task ShouldRunDotNetRestore()
    {
        var testing = new ModuleTesting<RestoreModule>();

        var summary = await testing.GetSummaryAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetRestoreOptions { ProjectSolution = Repo.Solution };

        testing.DotNet.Restore(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new ModuleTesting<RestoreModule>();

        testing
            .DotNet.Restore(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Restore failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.GetSummaryAsync);
        ex.Message.ShouldContain(nameof(RestoreModule));
    }
}
