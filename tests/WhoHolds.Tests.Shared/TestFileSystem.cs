using TUnit.Core.Interfaces;

namespace WhoHolds.Tests.Shared;

public sealed class TestFileSystem : IAsyncInitializer, IAsyncDisposable
{
    public string TempDir
    {
        get =>
            field
            ?? throw new InvalidOperationException(
                $"{nameof(TestFileSystem)}.{nameof(InitializeAsync)} must be called first."
            );
        private set;
    } = null!;

    private readonly bool _useTempSubdirectory = true;

    public TestFileSystem() { }

    public TestFileSystem(bool useTempSubdirectory)
    {
        _useTempSubdirectory = useTempSubdirectory;
    }

    public Task InitializeAsync()
    {
        TempDir = _useTempSubdirectory
            ? Directory.CreateTempSubdirectory("who-holds-tests-").FullName
            : Directory
                .CreateDirectory(
                    Path.Combine(
                        Environment.CurrentDirectory,
                        $"who-holds-tests-{Guid.NewGuid():N}"
                    )
                )
                .FullName;

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        try
        {
            Directory.Delete(TempDir, recursive: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete temp test folder: {ex.Message} at \"{TempDir}\".");
        }

        return ValueTask.CompletedTask;
    }

    public string CreateTestFile(string? name = null)
    {
        var path = Path.Combine(TempDir, name ?? Guid.NewGuid().ToString("N"));
        File.Create(path).Dispose();
        return path;
    }

    public string CreateSubdirectory(string? name = null)
    {
        var path = Path.Combine(TempDir, name ?? Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    public static FileStream HoldFile(string filePath)
    {
        return new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }
}
