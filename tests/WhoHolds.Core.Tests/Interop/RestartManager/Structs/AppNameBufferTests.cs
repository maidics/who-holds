using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

[InheritsTests]
internal sealed class AppNameBufferTests() : NativeStructTestBase<AppNameBuffer>(512)
{
    [Test]
    public void ShouldBeInlineArrayOfUshort()
    {
        AssertInlineArrayAttributeAndArrayType(
            RestartManagerLimits.MaxAppNameBufferLength,
            typeof(ushort)
        );
    }
}
