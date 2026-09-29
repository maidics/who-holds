using System.Reflection;
using ModularPipelines.Configuration;
using ModularPipelines.Modules;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class ModuleExtensions
{
    extension<T>(Module<T> module)
    {
        public ModuleConfiguration GetConfiguration()
        {
            var configure = typeof(Module<T>).GetMethod(
                "Configure",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            configure.ShouldNotBeNull();

            return (ModuleConfiguration)
                configure.Invoke(
                    module,
                    BindingFlags.DoNotWrapExceptions,
                    binder: null,
                    [],
                    culture: null
                )!;
        }
    }

    extension<TModule>(TModule)
        where TModule : class, IModule
    {
        public static ModularPipelines.Attributes.DependsOnAttribute<TDependency> ShouldHaveDependsOnAttribute<TDependency>()
            where TDependency : class, IModule
        {
            var attr =
                typeof(TModule).GetCustomAttribute<ModularPipelines.Attributes.DependsOnAttribute<TDependency>>();
            attr.ShouldNotBeNull();
            return attr;
        }
    }
}
