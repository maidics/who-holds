using ModularPipelines.Context;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class TestCppBuildToolLocator(CppBuildToolLookupResult result) : ICppBuildToolLocator
{
    public Task<CppBuildToolLookupResult> LocateAsync(IPipelineContext context) =>
        Task.FromResult(result);
}
