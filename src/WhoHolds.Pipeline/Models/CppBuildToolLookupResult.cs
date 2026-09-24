namespace WhoHolds.Pipeline.Models;

public sealed record CppBuildToolLookupResult(bool VsWhereFound, string? InstallationPath);
