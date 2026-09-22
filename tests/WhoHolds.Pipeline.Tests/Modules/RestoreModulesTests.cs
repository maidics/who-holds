using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests : DotNetModuleTestBase<RestoreModule>
{
    [Test]
    public override async Task ShouldRunModule()
    {
        var summary = await BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetRestoreOptions { ProjectSolution = Repo.Solution };

        _dotNet.Restore(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public override async Task ShouldFailPipelineWhenModuleFails()
    {
        _dotNet
            .Restore(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Restore failed."));

        var ex = await Should.ThrowAsync<Exception>(BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(RestoreModule));
    }
}
