namespace WhoHolds.Core.Tests.TestInfrastructure;

internal abstract class FileHandlerTestBase
{
    private static string _tempDir = null!;

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

    protected static string CreateTestFile(string? name = null) =>
        Path.Combine(_tempDir, name ?? Guid.NewGuid().ToString());
}
