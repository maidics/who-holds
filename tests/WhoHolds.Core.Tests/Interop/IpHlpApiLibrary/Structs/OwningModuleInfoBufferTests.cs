using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Constants;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class OwningModuleInfoBufferTests()
    : NativeStructTestBase<OwningModuleInfoBuffer>(
        IpHlpApiLimits.OwningModuleSize * Marshal.SizeOf<ulong>()
    )
{
    [Test]
    public void ShouldBeMarkedWithInlineArray()
    {
        AssertInlineArrayAttributeAndArrayType(IpHlpApiLimits.OwningModuleSize, typeof(ulong));
    }
}
