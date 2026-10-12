using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Core.Utility;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests.Utility;

internal sealed class PathUtilsTests
{
    [Test]
    public async Task ResolveFullPathShouldReturnFailureWhenPathIsADirectory()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var result = PathUtils.ResolveFullPath(fileSystem.TempDir);
        result.ShouldBeResultedTo(false, "Given path is a directory.");
    }

    [Test]
    public async Task ResolveFullPathShouldFailureIfFileNotFound()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var nonExisting = Path.Combine(fileSystem.TempDir, Guid.NewGuid().ToString());

        var result = PathUtils.ResolveFullPath(nonExisting);
        result.ShouldBeResultedTo(false, "File not found.");
    }

    // UnauthorizedAccessException is left out due to security reasons: TODO: implement it in november

    [Test]
    [Arguments("")]
    [Arguments("file\0.txt")]
    [Arguments("a<b.txt")] // illegal file name characters on Windows
    public void ResolveFullPathShouldReturnFailureForInvalidFilePaths(string path)
    {
        var result = PathUtils.ResolveFullPath(path);
        result.ShouldBeResultedTo(false, "Invalid file path.");
    }

    [Test]
    public async Task ShouldReturnSucceededForValidFullyQualifiedFilePath()
    {
        await using var fileSystem = new TestFileSystem();
        await fileSystem.InitializeAsync();

        var path = fileSystem.CreateTestFile();

        var result = PathUtils.ResolveFullPath(path);
        result.ShouldBeResultedTo(true);
        result.Value.ShouldBe(path);
    }

    [Test]
    public async Task ShouldReturnSucceededForValidRelativeFilePath()
    {
        await using var fileSystem = new TestFileSystem(false);
        await fileSystem.InitializeAsync();

        var file = fileSystem.CreateTestFile();

        var relative = Path.GetRelativePath(Environment.CurrentDirectory, file);
        Path.IsPathFullyQualified(relative).ShouldBeFalse();

        var result = PathUtils.ResolveFullPath(relative);
        result.ShouldBeResultedTo(true);
        result.Value.ShouldBe(file);
    }
}
