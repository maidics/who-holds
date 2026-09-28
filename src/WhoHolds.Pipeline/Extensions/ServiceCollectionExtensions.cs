using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPipelineSettings(IConfiguration configuration)
        {
            services
                .AddOptions<PipelineSettings>()
                .Bind(configuration.GetSection("Pipeline"))
                .Configure(settings =>
                {
                    settings.GitHubRefType = configuration["GITHUB_REF_TYPE"] ?? string.Empty;
                    settings.GitHubRefName = configuration["GITHUB_REF_NAME"] ?? string.Empty;
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public IServiceCollection AddPublishSettings(IConfiguration configuration)
        {
            services
                .AddOptions<PublishSettings>()
                .Bind(configuration.GetSection("Publish"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
