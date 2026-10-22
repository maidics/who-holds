using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

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
