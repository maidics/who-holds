using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests;

internal sealed class WhoHoldsTests
{
    [Test] // since this uses PathUtils covering one case is enough
    public async Task FileMethodToFileShouldReturnNotFoundIfFileNotFound()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var result = WhoHolds.File(Path.Combine(fileSystem.TempDir, Guid.NewGuid().ToString()));
        result.ShouldBeResultedTo(false, "File not found.");
    }

    [Test]
    public async Task FileMethodToFileShouldReturnEmptyArrayIfNothingHoldsFile()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var file = fileSystem.CreateTestFile();

        var result = WhoHolds.File(file);
        result.ShouldBeResultedTo(true);
        result.Value.Length.ShouldBe(0);
    }

    [Test] // because it orchestrates the search with RestartManagerSession covering one case where it returns something is enough as well
    public async Task FileMethodToFileShouldReturnHolderProcessesToAbsolutePath()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var file = fileSystem.CreateTestFile();
        await using var hold = TestFileSystem.HoldFile(file);

        var result = WhoHolds.File(file);
        result.ShouldBeResultedTo(true);

        var processes = result.Value;
        processes.Length.ShouldBe(1);

        var process = processes[0];
        process.ProcessId.ShouldBe(Environment.ProcessId);
        process.ImagePath.ShouldEndWith("dotnet.exe");
    }

    [Test]
    public async Task FileMethodToFileShouldReturnHolderProcessesToRelativePath()
    {
        // TestFileSystem uses %TEMP%, which can be on another drive than the working directory
        // (e.g. C: vs D: on CI runners); no relative path exists across drives.
        await using var fileSystem = new TestFileSystem(false);
        await fileSystem.InitializeAsync();

        var file = fileSystem.CreateTestFile();

        var relative = Path.GetRelativePath(Environment.CurrentDirectory, file);
        Path.IsPathFullyQualified(relative).ShouldBeFalse();

        await using var hold = TestFileSystem.HoldFile(file);

        var result = WhoHolds.File(relative);
        result.ShouldBeResultedTo(true);

        var processes = result.Value;
        processes.Length.ShouldBe(1);

        var process = processes[0];
        process.ProcessId.ShouldBe(Environment.ProcessId);
        process.ImagePath.ShouldEndWith("dotnet.exe");
    }
}
