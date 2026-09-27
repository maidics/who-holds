using System.ComponentModel.DataAnnotations;

namespace WhoHolds.Pipeline.Settings;

public sealed record PipelineSettings // TODO: remove ConfigurationRequirement
{
    [Required]
    public string Configuration { get; set; } = string.Empty;

    [Required]
    public string GitHubRefType { get; set; } = string.Empty;

    public bool IsTagPush => GitHubRefType == GitHubTagRef;

    [Required]
    public string GitHubRefName { get; set; } = string.Empty;

    [Required]
    public string GitHubTagRef { get; set; } = string.Empty;

    public string Solution { get; } = nameof(WhoHolds) + ".slnx";
}
