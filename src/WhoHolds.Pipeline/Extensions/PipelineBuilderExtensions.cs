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
        public PipelineBuilder ConfigurePipeline(string appSettings)
        {
            builder.Configuration.AddJsonFile(appSettings, optional: false);

            builder.ConfigurePipelineOptions(options =>
            {
                options.PrintLogo = false;
                options.ShowProgressInConsole = true;
                options.ExecutionMode = ExecutionMode.StopOnFirstException;
            });

            return builder;
        }

        public PipelineBuilder AddServices()
        {
            builder.Services.AddSingleton<ICppBuildToolsLocator>(_ => new CppBuildToolsLocator(
                CppBuildToolsLocator.DefaultPath
            ));

            return builder;
        }

        public PipelineBuilder AddGlobalHooks()
        {
            return builder.AddPipelineGlobalHooks<PipelineInformationHooks>();
        }

        public PipelineBuilder AddRequirements()
        {
            builder
                .AddRequirement<WindowsRequirement>()
                .AddRequirement<VersionTagFormatRequirement>()
                .AddRequirement<CppBuildToolsRequirement>();

            return builder;
        }

        public PipelineBuilder AddModules()
        {
            return builder
                .AddModule<RestoreModule>()
                .AddModule<BuildModule>()
                .AddModule<TestModule>(); // TODO: add other modules
        }
    }
}
