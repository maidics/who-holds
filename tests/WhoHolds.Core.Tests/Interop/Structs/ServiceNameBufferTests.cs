using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

[InheritsTests]
internal sealed class ServiceNameBufferTests() : NativeStructTestBase<ServiceNameBuffer>(128)
{
    [Test]
    public void ShouldBeInlineArrayOfUshort()
    {
        AssertInlineArrayAttributeAndArrayType(
            RestartManagerLimits.ServiceNameBufferLength,
            typeof(ushort)
        );
    }
}
