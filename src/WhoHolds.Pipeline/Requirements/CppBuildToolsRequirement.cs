using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Options;
using ModularPipelines.Requirements;

namespace WhoHolds.Pipeline.Requirements;

public sealed class CppBuildToolsRequirement : IPipelineRequirement
{
    public async Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        var vswhere = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            "Microsoft Visual Studio",
            "Installer",
            "vswhere.exe"
        );

        if (!File.Exists(vswhere))
            return RequirementDecision.Failed(
                "vswhere not found. Install Visual Studio or Build Tools with the 'Desktop development with C++' workload."
            );

        var options = new GenericCommandLineToolOptions(vswhere)
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
        };

        var result = await context.Shell.Command.ExecuteCommandLineTool(options);

        if (string.IsNullOrWhiteSpace(result.StandardOutput))
        {
            return RequirementDecision.Failed(
                "C++ build tools (MSVC x64) not found. Native AOT publish requires the 'Desktop development with C++' workload."
            );
        }

        return RequirementDecision.Passed;
    }
}
