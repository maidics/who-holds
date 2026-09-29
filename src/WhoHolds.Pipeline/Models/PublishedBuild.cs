namespace WhoHolds.Pipeline.Models;

public sealed record PublishedBuild(string FileName, string Directory, string Version)
{
    // Passed between modules: a relative path would be resolved against whatever the current
    // directory happens to be at the time of use, so only fully qualified paths are accepted.
    public string Directory { get; init; } =
        Path.IsPathFullyQualified(Directory)
            ? Directory
            : throw new ArgumentException(
                $"Published build directory must be an absolute path, got: '{Directory}'.",
                nameof(Directory)
            );

    public string FilePath => Path.Combine(Directory, FileName);
}
