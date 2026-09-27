using System.Reflection;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Modules;

namespace WhoHolds.Pipeline.Tests.Extensions;

public static class ModuleExtensions
{
    extension<T>(Module<T> module)
    {
        public Task<T?> TestExecuteAsync(
            IModuleContext context,
            CancellationToken cancellationToken = default
        )
        {
            var execute = typeof(Module<T>).GetMethod(
                "ExecuteAsync",
                BindingFlags.Instance | BindingFlags.NonPublic,
                [typeof(IModuleContext), typeof(CancellationToken)]
            );
            execute.ShouldNotBeNull();

            var task = execute.Invoke(
                module,
                BindingFlags.DoNotWrapExceptions,
                binder: null,
                [context, cancellationToken],
                culture: null
            );

            return (Task<T?>)task!;
        }
    }

    extension<TModule>(TModule) where TModule : class, IModule
    {
        public static ModularPipelines.Attributes.DependsOnAttribute<TDependency> ShouldHaveDependsOnAttribute<
            TDependency
        >()
            where TDependency : class, IModule
        {
            var attr =
                typeof(TModule).GetCustomAttribute<ModularPipelines.Attributes.DependsOnAttribute<TDependency>>();
            attr.ShouldNotBeNull();
            return attr;
        }
    }
}
