using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class ModuleTesting<TModule>
    where TModule : class, IModule
{
    public PipelineBuilder PipelineBuilder { get; }
    public IDotNetMock DotNet { get; }

    public ModuleTesting(Func<IServiceProvider, TModule> factory)
    {
        PipelineBuilder = ModularPipelines.Pipeline.CreateBuilder();
        DotNet = IDotNet.Mock();
        PipelineBuilder.Services.AddSingleton(DotNet.Object);
        PipelineBuilder.AddModule(factory);
    }

    public ModuleTesting()
    {
        PipelineBuilder = ModularPipelines.Pipeline.CreateBuilder();
        DotNet = IDotNet.Mock();
        PipelineBuilder.Services.AddSingleton(DotNet.Object);
        PipelineBuilder.AddModule<TModule>();
    }

    public void AddModuleDependency<TModuleDependency>(
        params Func<IServiceProvider, TModuleDependency>[] dependsOn
    )
        where TModuleDependency : class, IModule
    {
        foreach (var d in dependsOn)
        {
            PipelineBuilder.AddModule(d);
        }
    }

    public async Task<PipelineSummary> GetSummaryAsync()
    {
        await using var pipeline = await PipelineBuilder.BuildAsync();

        return await pipeline.RunAsync();
    }

    public static void ShouldNotDependOnAnyModule()
    {
        var attributes = typeof(TModule)
            .GetCustomAttributes(inherit: true)
            .Where(a => a.GetType().Name.StartsWith(nameof(DependsOnAttribute)));

        attributes.ShouldBeEmpty();
    }

    public static ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency> ShouldDependOn<TModuleDependency>()
        where TModuleDependency : class, IModule
    {
        var attr =
            typeof(TModule).GetCustomAttribute<ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency>>();

        attr.ShouldNotBeNull();

        return attr;
    }
}
