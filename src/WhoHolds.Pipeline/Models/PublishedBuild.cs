namespace WhoHolds.Pipeline.Models;

public sealed record PublishedBuild(string FileName, string Directory, string Version)
{
    public string FilePath => Path.GetFullPath(Path.Combine(Directory, FileName));
}
