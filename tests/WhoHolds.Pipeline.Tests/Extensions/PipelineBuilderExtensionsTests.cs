using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Interfaces;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.GlobalHooks;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Services;

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
    public void ShouldAddServices()
    {
        _builder.AddServices();

        // provider has to be built because CppBuildToolLocator was registered with a factory
        using var provider = _builder.Services.BuildServiceProvider();

        provider.GetRequiredService<ICppBuildToolLocator>().ShouldBeOfType<CppBuildToolLocator>();
        provider
            .GetRequiredService<IReleaseVersionResolver>()
            .ShouldBeOfType<ReleaseVersionResolver>();
    }

    [Test]
    public void ShouldAddGlobalHooks()
    {
        _builder.AddGlobalHooks();

        ContainsService<IPipelineGlobalHooks, LoggingGlobalHooks>().ShouldBeTrue();
    }

    [Test]
    public void ShouldAddRequirements()
    {
        _builder.AddRequirements();

        _builder.Services.Count(d => d.ServiceType == typeof(IPipelineRequirement)).ShouldBe(4);

        ContainsService<IPipelineRequirement, WindowsRequirement>().ShouldBeTrue();
        ContainsService<IPipelineRequirement, ConfigurationRequirement>().ShouldBeTrue();
        ContainsService<IPipelineRequirement, VersionTagFormatRequirement>().ShouldBeTrue();
        ContainsService<IPipelineRequirement, CppBuildToolRequirement>().ShouldBeTrue();
    }

    [Test]
    public async Task ShouldAddModules()
    {
        _builder.AddModules();

        List<Type> expected = [typeof(RestoreModule), typeof(BuildModule), typeof(TestModule)];

        await using var pipeline = await _builder.BuildAsync();

        pipeline.Services.GetServices<IModule>().Select(m => m.GetType()).ShouldBe(expected);
    }

    private bool ContainsService<TService, TImplementation>()
        where TService : notnull
        where TImplementation : TService =>
        _builder.Services.Any(d =>
            d.ServiceType == typeof(TService) && d.ImplementationType == typeof(TImplementation)
        );
}
