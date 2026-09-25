using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests;

internal sealed class WhoHoldsTests
{
    [ClassDataSource<TestFileSystem>]
    public required TestFileSystem Testing { get; init; }

    [Test] // since this uses PathUtils covering one case is enough
    public void FileMethodToFileShouldReturnNotFoundIfFileNotFound()
    {
        var result = WhoHolds.File(Path.Combine(Testing.TempDir, Guid.NewGuid().ToString()));
        result.ShouldBeResultedTo(false, "File not found.");
    }

    [Test]
    public void FileMethodToFileShouldReturnEmptyArrayIfNothingHoldsFile()
    {
        var file = Testing.CreateTestFile();

        var result = WhoHolds.File(file);
        result.ShouldBeResultedTo(true);
        result.Value.Length.ShouldBe(0);
    }

    [Test] // because it orchestrates the search with RestartManagerSession covering one case where it returns something is enough as well
    public void FileMethodToFileShouldReturnHolderProcesses()
    {
        var file = Testing.CreateTestFile();
        using var hold = TestFileSystem.HoldFile(file);

        var result = WhoHolds.File(file);
        result.ShouldBeResultedTo(true);

        var processes = result.Value;
        processes.Length.ShouldBe(1);
        processes[0].ProcessId.ShouldBe(Environment.ProcessId);
    }
}
