using WhoHolds.Pipeline.Interfaces;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class FakeReleaseVersionResolver(string version) : IReleaseVersionResolver
{
    public string Resolve(string tagName) => version;
}
