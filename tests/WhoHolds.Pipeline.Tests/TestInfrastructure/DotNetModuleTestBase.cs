using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public abstract class DotNetModuleTestBase<TModule>
    where TModule : class, IModule
{
    protected readonly IDotNetMock _dotNet;
    protected readonly PipelineBuilder _builder;

    protected DotNetModuleTestBase()
    {
        _dotNet = IDotNet.Mock();
        _builder = ModularPipelines.Pipeline.CreateBuilder();
        _builder.Services.AddSingleton(_dotNet.Object);
        _builder.AddModule<TModule>();
    }

    protected async Task<PipelineSummary> BuildAndRunAsync() // runs using the IDotNet mock so the module does not actually run
    {
        var pipeline = await _builder.BuildAsync();
        return await pipeline.RunAsync();
    }

    protected ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency> ShouldHaveDependsOnAttribute<TModuleDependency>()
        where TModuleDependency : class, IModule
    {
        var attr =
            typeof(TModule).GetCustomAttribute<ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency>>();
        attr.ShouldNotBeNull();
        return attr;
    }

    public abstract Task ShouldRunModule();

    public abstract Task ShouldFailPipelineWhenModuleFails();
}
