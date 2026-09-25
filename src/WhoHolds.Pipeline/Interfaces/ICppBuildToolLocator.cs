using ModularPipelines.Context;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Interfaces;

public interface ICppBuildToolLocator
{
    Task<CppBuildToolLookupResult> LocateAsync(IPipelineContext context); // to use context.Shell.Command
}
