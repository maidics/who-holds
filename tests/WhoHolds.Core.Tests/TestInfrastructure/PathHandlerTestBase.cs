namespace WhoHolds.Core.Tests.TestInfrastructure;

[NotInParallel] // because of static _tempDir
internal abstract class PathHandlerTestBase
{
    protected static string _tempDir = null!;

    [Before(Class)]
    public static void SetUp()
    {
        _tempDir = Directory.CreateTempSubdirectory("rm-tests-").FullName;
    }

    [After(Class)]
    public static void TearDown()
    {
        try
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Failed to delete temp test folder: {ex.Message} at \"{_tempDir}\"."
            );
        }
    }

    protected static string CreateTestFile(string? name = null)
    {
        var path = Path.Combine(_tempDir, name ?? Guid.NewGuid().ToString("N"));
        File.Create(path).Dispose();
        return path;
    }

    protected static FileStream HoldFile(string filePath)
    {
        return new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }
}
