using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class PipelineTesting
{
    public IDotNetMock Dotnet { get; }
    public PipelineBuilder Builder { get; }

    public PipelineTesting(params Func<IServiceProvider, IModule>[] moduleFactories)
    {
        Dotnet = IDotNet.Mock();
        Builder = ModularPipelines.Pipeline.CreateBuilder();
        Builder.Services.AddSingleton(Dotnet.Object);

        foreach (var factory in moduleFactories)
        {
            Builder.AddModule(factory);
        }
    }

    public async Task<PipelineSummary> BuildAndRunAsync() // runs using the IDotNet mock so the module does not actually run
    {
        var pipeline = await Builder.BuildAsync();
        return await pipeline.RunAsync();
    }

    public static ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency> ShouldHaveDependsOnAttribute<
        TModule,
        TModuleDependency
    >()
        where TModule : class, IModule
        where TModuleDependency : class, IModule
    {
        var attr =
            typeof(TModule).GetCustomAttribute<ModularPipelines.Attributes.DependsOnAttribute<TModuleDependency>>();
        attr.ShouldNotBeNull();
        return attr;
    }
}
