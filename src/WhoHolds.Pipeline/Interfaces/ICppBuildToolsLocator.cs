using ModularPipelines.Context;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Interfaces;

public interface ICppBuildToolsLocator
{
    Task<CppBuildToolsLookupResult> LocateAsync(IPipelineContext context); // to use context.Shell.Command
}
