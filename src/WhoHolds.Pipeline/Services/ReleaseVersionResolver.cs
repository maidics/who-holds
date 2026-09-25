using WhoHolds.Pipeline.Interfaces;

namespace WhoHolds.Pipeline.Services;

public sealed class ReleaseVersionResolver : IReleaseVersionResolver // TODO: add tests, register in AddServices, test AddServices
{
    public string Resolve(string tagName) // TagVersionRequirement ensures the tag name is the correct format
    {
        if (!Version.TryParse(tagName.TrimStart('v'), out var version))
            throw new ArgumentException($"Failed to parse tag name: '{tagName}'.");

        return version.ToString();
    }
}
