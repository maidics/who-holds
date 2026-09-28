using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.Extensions;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests
{
    private static readonly IOptions<PipelineSettings> _options = Options.Create(
        new PipelineSettings()
    );

    [Test]
    public async Task ShouldRunModule()
    {
        var dotnet = IDotNet.Mock();
        var context = IModuleContext.CreateWithDotNetMock(dotnet);
        var module = new RestoreModule(_options);
        await module.TestExecuteAsync(context);

        var expectedOptions = new DotNetRestoreOptions
        {
            ProjectSolution = _options.Value.Solution,
        };

        dotnet.Restore(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var dotnet = IDotNet.Mock();
        var context = IModuleContext.CreateWithDotNetMock(dotnet);
        var module = new RestoreModule(_options);

        var exception = new InvalidOperationException("Restore failed.");
        dotnet.Restore(Any(), Any(), Any()).Throws(exception);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            module.TestExecuteAsync(context)
        ); // ignore error
        ex.Message.ShouldBe(exception.Message);
    }
}
