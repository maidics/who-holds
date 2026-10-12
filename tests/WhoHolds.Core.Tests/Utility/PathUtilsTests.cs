using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Core.Utility;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests.Utility;

internal sealed class PathUtilsTests
{
    [ClassDataSource<TestFileSystem>]
    public required TestFileSystem Testing { get; init; }

    [Test]
    public void ResolveFullPathShouldReturnFailureWhenPathIsADirectory()
    {
        var result = PathUtils.ResolveFullPath(Testing.TempDir);
        result.ShouldBeResultedTo(false, "Given path is a directory.");
    }

    [Test]
    public void ResolveFullPathShouldFailureIfFileNotFound()
    {
        var nonExisting = Path.Combine(Testing.TempDir, Guid.NewGuid().ToString());

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
    public void ShouldReturnSucceededForValidFullyQualifiedFilePath()
    {
        var path = Testing.CreateTestFile();

        var result = PathUtils.ResolveFullPath(path);
        result.ShouldBeResultedTo(true);
    }
}
