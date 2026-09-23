namespace WhoHolds.Pipeline.Models;

public sealed record CppBuildToolsLookupResult(bool VsWhereFound, string? InstallationPath);
