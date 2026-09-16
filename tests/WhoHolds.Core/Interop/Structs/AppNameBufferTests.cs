using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

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
