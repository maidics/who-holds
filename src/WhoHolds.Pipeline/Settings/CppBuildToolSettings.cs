using System.ComponentModel.DataAnnotations;

namespace WhoHolds.Pipeline.Settings;

public sealed record CppBuildToolSettings
{
    [Required]
    public string VisualStudioSubfolder { get; set; } = string.Empty;

    [Required]
    public string InstallerSubfolder { get; set; } = string.Empty;

    [Required]
    public string VsWhere { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string[] VsWhereArguments { get; set; } = null!;

    public string ProgramFilesX86 { get; set; } =
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

    public string VsWhereFullPath =>
        Path.Combine(ProgramFilesX86, VisualStudioSubfolder, InstallerSubfolder, VsWhere);
}
