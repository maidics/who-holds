using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests
{
    private readonly PipelineTesting _testing = new(_ => new RestoreModule());

    [Test]
    public async Task ShouldRunModule()
    {
        var summary = await _testing.BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetRestoreOptions { ProjectSolution = Repo.Solution };

        _testing.Dotnet.Restore(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        _testing
            .Dotnet.Restore(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Restore failed."));

        var ex = await Should.ThrowAsync<Exception>(_testing.BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(RestoreModule));
    }
}
