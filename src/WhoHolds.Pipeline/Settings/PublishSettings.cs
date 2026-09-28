using System.ComponentModel.DataAnnotations;

namespace WhoHolds.Pipeline.Settings;

public sealed record PublishSettings
{
    [Required]
    public string Runtime { get; set; } = string.Empty;

    [Required]
    public string OutputDirectory { get; set; } = string.Empty;

    [Required]
    public string ProjectPath { get; set; } = string.Empty;
}
