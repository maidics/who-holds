using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Extensions;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
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
            builder.ConfigurePipelineOptions(options =>
            {
                options.PrintLogo = false;
                options.ShowProgressInConsole = true;
                options.ExecutionMode = ExecutionMode.StopOnFirstException;
            });

            return builder;
        }

        public PipelineBuilder AddRequirements(bool isTagPush)
        {
            builder.AddRequirement<WindowsRequirement>();

            if (isTagPush)
            {
                builder.Services.AddSingleton<ICppBuildToolsLocator>(_ => new CppBuildToolsLocator(
                    CppBuildToolsLocator.DefaultPath
                ));

                builder.AddRequirement<CppBuildToolsRequirement>();
            }

            return builder;
        }

        public PipelineBuilder AddModules(string? releaseVersion)
        {
            builder
                .AddModule<RestoreModule>()
                .AddModule(_ => new BuildModule(releaseVersion))
                .AddModule<TestModule>();

            // TODO: add required modules

            return builder;
        }
    }
}
