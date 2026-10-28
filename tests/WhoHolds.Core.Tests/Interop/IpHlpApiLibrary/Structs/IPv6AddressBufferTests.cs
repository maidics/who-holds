using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Constants;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class IPv6AddressBufferTests()
    : NativeStructTestBase<IPv6AddressBuffer>(
        IpHlpApiLimits.IPv6AddressLength * Marshal.SizeOf<byte>()
    )
{
    [Test]
    public void ShouldBeMarkedWithInlineArray()
    {
        AssertInlineArrayAttributeAndArrayType(IpHlpApiLimits.IPv6AddressLength, typeof(byte));
    }
}
