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
using WhoHolds.Pipeline.Settings;

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

        public PipelineBuilder AddSettings()
        {
            return builder.AddPipelineSettings().AddPublishSettings();
        }

        public PipelineBuilder AddPipelineSettings()
        {
            var config = builder.Configuration;

            builder
                .Services.AddOptions<PipelineSettings>()
                .Bind(config.GetSection("Pipeline"))
                .Configure(settings =>
                {
                    settings.GitHubRefType = config["GITHUB_REF_TYPE"] ?? string.Empty;
                    settings.GitHubRefName = config["GITHUB_REF_NAME"] ?? string.Empty;
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return builder;
        }

        public PipelineBuilder AddPublishSettings()
        {
            var config = builder.Configuration;

            builder
                .Services.AddOptions<PublishSettings>()
                .Bind(config.GetSection("Publish"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return builder;
        }

        public PipelineBuilder AddServices()
        {
            builder.Services.AddSingleton<ICppBuildToolLocator>(_ => new CppBuildToolLocator(
                CppBuildToolLocator.DefaultPath
            ));

            builder.Services.AddSingleton<IReleaseVersionResolver, ReleaseVersionResolver>();

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
