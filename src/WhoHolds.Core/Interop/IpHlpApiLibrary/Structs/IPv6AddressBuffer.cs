using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Constants;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

// this can be read via new IPAddress(buffer, localScopeId)
// localScopeId identifies the network adapter on the OS the IPv6 belongs to
[StructLayout(LayoutKind.Sequential)]
[InlineArray(IpHlpApiLimits.IPv6AddressLength)]
internal struct IPv6AddressBuffer : INativeSized<IPv6AddressBuffer>
{
    private byte _element0;

    public static int Size => Marshal.SizeOf<IPv6AddressBuffer>();
}
