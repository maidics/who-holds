using ModularPipelines.Context;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class TestCppBuildToolsLocator(CppBuildToolsLookupResult result)
    : ICppBuildToolsLocator
{
    public Task<CppBuildToolsLookupResult> LocateAsync(IPipelineContext context) =>
        Task.FromResult(result);
}
