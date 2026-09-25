using WhoHolds.Pipeline.Services;

namespace WhoHolds.Pipeline.Tests.Services;

public sealed class ReleaseVersionResolverTests
{
    [Test]
    [Arguments("")]
    [Arguments("\n")]
    [Arguments("test")]
    [Arguments("v1.0.0-pre")]
    public void ShouldThrowArgumentExceptionIfParsingFails(string invalid)
    {
        var resolver = new ReleaseVersionResolver();

        Should.Throw<ArgumentException>(() => resolver.Resolve(invalid));
    }

    [Test]
    [Arguments("v1.0.0")]
    [Arguments("v5.0.0")]
    public void ShouldResolveReleaseVersion(string tag)
    {
        var resolver = new ReleaseVersionResolver();

        var version = resolver.Resolve(tag);
        version.ShouldBe(tag.TrimStart('v'));
    }
}
