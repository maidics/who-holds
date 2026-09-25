using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Extensions;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.GlobalHooks;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Services;

namespace WhoHolds.Pipeline.Extensions;

public static class PipelineBuilderExtensions
{
    extension(PipelineBuilder builder)
    {
        public PipelineBuilder ConfigurePipeline()
        {
            return builder.ConfigurePipelineOptions(options =>
            {
                options.PrintLogo = false;
                options.ShowProgressInConsole = true;
                options.ExecutionMode = ExecutionMode.StopOnFirstException;
            });
        }

        public PipelineBuilder AddJsonConfiguration()
        {
            builder.Configuration.AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: false
            );

            return builder;
        }

        public PipelineBuilder AddServices()
        {
            builder.Services.AddSingleton<ICppBuildToolLocator>(_ => new CppBuildToolLocator(
                CppBuildToolLocator.DefaultPath
            ));

            return builder;
        }

        public PipelineBuilder AddGlobalHooks()
        {
            return builder.AddPipelineGlobalHooks<LoggingGlobalHooks>();
        }

        public PipelineBuilder AddRequirements()
        {
            return builder
                .AddRequirement<WindowsRequirement>()
                .AddRequirement<ConfigurationRequirement>()
                .AddRequirement<VersionTagFormatRequirement>()
                .AddRequirement<CppBuildToolRequirement>();
        }

        public PipelineBuilder AddModules()
        {
            return builder
                .AddModule<RestoreModule>()
                .AddModule<BuildModule>()
                .AddModule<TestModule>();
        }
    }
}
