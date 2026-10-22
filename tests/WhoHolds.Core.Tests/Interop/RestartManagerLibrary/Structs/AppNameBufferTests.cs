using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.RestartManagerLibrary.Structs;

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
