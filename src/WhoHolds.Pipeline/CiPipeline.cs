using System.Runtime.InteropServices;
using ModularPipelines.Extensions;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Modules;

namespace WhoHolds.Pipeline;

public static class CiPipeline
{
    public static readonly IPipelineRequirement WindowsRequirement = Require.Platform(
        OSPlatform.Windows
    );

    extension(PipelineBuilder builder)
    {
        public PipelineBuilder AddCi()
        {
            builder.AddRequirement(WindowsRequirement);

            builder.ConfigurePipelineOptions(options =>
            {
                options.PrintLogo = false;
                options.ShowProgressInConsole = true;
                options.ExecutionMode = ExecutionMode.StopOnFirstException;
            });

            builder.AddModule<RestoreModule>().AddModule<BuildModule>(); //TODO: add more modules

            return builder;
        }
    }
}
