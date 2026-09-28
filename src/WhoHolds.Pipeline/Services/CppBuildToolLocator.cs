using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Options;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Services;

public sealed class CppBuildToolLocator(IOptions<CppBuildToolSettings> options)
    : ICppBuildToolLocator
{
    public async Task<CppBuildToolLookupResult> LocateAsync(IPipelineContext context)
    {
        if (!File.Exists(options.Value.VsWhereFullPath))
        {
            return new CppBuildToolLookupResult(VsWhereFound: false, InstallationPath: null);
        }

        var result = await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(options.Value.VsWhereFullPath)
            {
                Arguments = options.Value.VsWhereArguments,
            }
        );

        var path = result.StandardOutput.Trim();

        return new CppBuildToolLookupResult(
            VsWhereFound: true,
            string.IsNullOrEmpty(path) ? null : path
        );
    }
}
