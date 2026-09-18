using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests;

internal sealed class HandleFinderTests : PathHandlerTestBase
{
    [Test] // since this uses PathUtils covering one case is enough
    public void GetHolderProcessesToFileShouldReturnNotFoundIfFileNotFound()
    {
        var result = HandleFinder.GetHolderProcessesToFile(
            Path.Combine(_tempDir, Guid.NewGuid().ToString())
        );
        result.Type.ShouldBe(ResultType.NotFound);
    }

    [Test]
    public void GetHolderProcessesToFileShouldReturnEmptyArrayIfNothingHoldsFile()
    {
        var file = CreateTestFile();

        var result = HandleFinder.GetHolderProcessesToFile(file);
        result.ShouldBeResultedTo(ResultType.Success);
        result.Value.Length.ShouldBe(0);
    }

    [Test] // because it orchestrates the search with RestartManagerSession covering one case where it returns something is enough as well
    public void GetHolderProcessesToFileShouldReturnHolderProcesses()
    {
        var file = CreateTestFile();
        using var hold = HoldFile(file);

        var result = HandleFinder.GetHolderProcessesToFile(file);
        result.ShouldBeResultedTo(ResultType.Success);

        var processes = result.Value;
        processes.Length.ShouldBe(1);
        processes[0].ProcessId.ShouldBe(Environment.ProcessId);
    }
}
