using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.RestartManagerLibrary.Structs;

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
