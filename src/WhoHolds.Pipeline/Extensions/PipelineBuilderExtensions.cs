using Microsoft.Extensions.Configuration;
using ModularPipelines;
using ModularPipelines.Extensions;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Requirements;

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

        public PipelineBuilder AddRequirements()
        {
            return builder
                .AddRequirement<WindowsRequirement>()
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
