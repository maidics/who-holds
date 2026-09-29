using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class RestoreModulesTests
{
    [Test]
    public async Task RestoreAsyncShouldRestore()
    {
        var dotnet = IDotNet.Mock();

        const string solution = nameof(solution);

        var expectedOptions = new DotNetRestoreOptions { ProjectSolution = solution };

        await RestoreModule.RestoreAsync(dotnet, solution, CancellationToken.None);

        dotnet.Restore(expectedOptions, Any(), Any()).WasCalled(Times.Once);
    }
}
