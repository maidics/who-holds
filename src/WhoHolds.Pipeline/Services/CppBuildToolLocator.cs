using ModularPipelines.Context;
using ModularPipelines.Options;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Services;

public sealed class CppBuildToolsLocator(string vswherePath) : ICppBuildToolsLocator
{
    public static string DefaultPath { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            "Microsoft Visual Studio",
            "Installer",
            Repo.VsWhere
        );

    public async Task<CppBuildToolsLookupResult> LocateAsync(IPipelineContext context)
    {
        if (!File.Exists(vswherePath))
        {
            return new CppBuildToolsLookupResult(VsWhereFound: false, InstallationPath: null);
        }

        var result = await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(vswherePath)
            {
                Arguments =
                [
                    "-latest",
                    "-products",
                    "*",
                    "-requires",
                    "Microsoft.VisualStudio.Component.VC.Tools.x86.x64",
                    "-property",
                    "installationPath",
                ],
            }
        );

        var path = result.StandardOutput.Trim();

        return new CppBuildToolsLookupResult(
            VsWhereFound: true,
            string.IsNullOrEmpty(path) ? null : path
        );
    }
}
