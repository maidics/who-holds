using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.Architecture;

internal sealed class InteropInternalTests
{
    private static readonly string _namespace = typeof(RestartManagerSession).Namespace!;

    [Test]
    public void AllInteropTypesShouldBeInternal()
    {
        var assembly = typeof(RestartManagerSession).Assembly;

        var publicTypes = assembly.GetExportedTypes().Where(IsInteropType);
        publicTypes.ShouldBeEmpty();
    }

    private static bool IsInteropType(Type t) =>
        t.Namespace is { } ns
        && (ns == _namespace || ns.StartsWith(_namespace + ".", StringComparison.Ordinal));
}
