using WhoHolds.Pipeline.Interfaces;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class FakeReleaseVersionResolver : IReleaseVersionResolver
{
    public string Resolve(string tagName) => tagName;
}
