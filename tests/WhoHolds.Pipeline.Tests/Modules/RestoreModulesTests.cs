using Microsoft.Extensions.Options;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests
{
    private static readonly IOptions<PipelineSettings> _options = Options.Create(
        new PipelineSettings()
    );

    private readonly PipelineTesting _testing = new(_ => new RestoreModule(_options));

    [Test]
    public async Task ShouldRunModule()
    {
        var summary = await _testing.BuildAndRunAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetRestoreOptions
        {
            ProjectSolution = _options.Value.Solution,
        };

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
