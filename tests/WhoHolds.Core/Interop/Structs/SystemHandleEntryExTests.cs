using System.Runtime.InteropServices;
using Shouldly;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core.Tests.Interop.Structs;

public sealed class SystemHandleEntryExTests
{
    [Test]
    public void SizeShouldMatchNativeSize()
    {
        Marshal.SizeOf<SystemHandleEntryEx>().ShouldBe(40);
    }
}
