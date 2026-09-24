using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Modules;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class PipelineBuilderExtensionTests
{
    private readonly PipelineBuilder _builder = ModularPipelines.Pipeline.CreateBuilder();

    [Test]
    public void ShouldConfigurePipeline()
    {
        _builder.ConfigurePipeline();

        _builder.Options.PrintLogo.ShouldBeFalse();
        _builder.Options.ShowProgressInConsole.ShouldBeTrue();
        _builder.Options.ExecutionMode.ShouldBe(ExecutionMode.StopOnFirstException);
    }

    [Test]
    public void ShouldAddJsonConfiguration()
    {
        // _builder does not respect .Environment.EnvironmentName
        // because of this the environment is set in Testing
        _builder.AddJsonConfiguration();

        var json = _builder.Configuration.Sources.OfType<JsonConfigurationSource>().Single();
        json.Path.ShouldBe("appsettings.Testing.json");
        json.Optional.ShouldBeFalse();
    }

    [Test]
    public void ShouldAddRequirements()
    {
        _builder.AddRequirements();

        _builder.Services.Count(d => d.ServiceType == typeof(IPipelineRequirement)).ShouldBe(1);

        ContainsRequirement<WindowsRequirement>().ShouldBeTrue();
    }

    [Test]
    public async Task ShouldAddModules()
    {
        _builder.AddModules();

        List<Type> expected = [typeof(RestoreModule), typeof(BuildModule), typeof(TestModule)];

        await using var pipeline = await _builder.BuildAsync();

        pipeline.Services.GetServices<IModule>().Select(m => m.GetType()).ShouldBe(expected);
    }

    private bool ContainsRequirement<TRequirement>()
        where TRequirement : IPipelineRequirement =>
        _builder.Services.Any(d =>
            d.ServiceType == typeof(IPipelineRequirement)
            && (d.ImplementationType ?? d.ImplementationInstance?.GetType()) == typeof(TRequirement)
        );
}
