using Microsoft.Extensions.Configuration.Json;
using ModularPipelines;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Requirements;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class PipelineBuilderExtensionTests
{
    private readonly PipelineBuilder _builder = ModularPipelines.Pipeline.CreateBuilder();

    [Test]
    public void ShouldConfigurePipeline()
    {
        const string appSettings = "appsettings.Testing.json";
        _builder.ConfigurePipeline(appSettings);

        _builder.Options.PrintLogo.ShouldBeFalse();
        _builder.Options.ShowProgressInConsole.ShouldBeTrue();
        _builder.Options.ExecutionMode.ShouldBe(ExecutionMode.StopOnFirstException);

        var json = _builder.Configuration.Sources.OfType<JsonConfigurationSource>().Single();
        json.Path.ShouldBe(appSettings);
        json.Optional.ShouldBeFalse();
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void ShouldAddRequirements(bool isTagPush)
    {
        _builder.AddRequirements(isTagPush);

        _builder
            .Services.Count(d => d.ServiceType == typeof(IPipelineRequirement))
            .ShouldBe(isTagPush ? 2 : 1);

        ContainsRequirement<WindowsRequirement>().ShouldBeTrue();
        ContainsRequirement<CppBuildToolsRequirement>().ShouldBe(isTagPush);
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task ShouldAddModules(bool isTagPush)
    {
        _builder.AddModules(isTagPush ? "test" : null);

        List<Type> expected = [typeof(RestoreModule), typeof(BuildModule), typeof(TestModule)];

        await using var pipeline = await _builder.BuildAsync();

        pipeline.Services.GetServices<IModule>().Select(m => m.GetType()).ShouldBe(expected);

        // TODO: add more checks for modules
    }

    private bool ContainsRequirement<TRequirement>()
        where TRequirement : IPipelineRequirement =>
        _builder.Services.Any(d =>
            d.ServiceType == typeof(IPipelineRequirement)
            && (d.ImplementationType ?? d.ImplementationInstance?.GetType()) == typeof(TRequirement)
        );
}
