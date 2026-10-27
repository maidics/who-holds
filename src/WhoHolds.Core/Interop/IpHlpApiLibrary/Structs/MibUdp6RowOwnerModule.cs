using System.Buffers.Binary;
using System.Net;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MibUdp6RowOwnerModule
    : INativeSized<MibUdp6RowOwnerModule>,
        IPortRowOwnerModule
{
    public readonly IPv6AddressBuffer ucLocalAddr;
    public readonly uint dwLocalScopeId;
    public readonly uint dwLocalPort;
    public readonly uint dwOwningPid;

    // 4 bytes padding
    public readonly long liCreateTimestamp;
    public readonly int dwFlags;

    // 4 bytes padding
    public readonly OwningModuleInfoBuffer OwningModuleInfo;

    public ushort LocalPort => BinaryPrimitives.ReverseEndianness((ushort)dwLocalPort);
    public IPAddress LocalAddress => new(ucLocalAddr, dwLocalScopeId);
    public bool SpecificPortBind => (dwFlags & 1) != 0;
    public DateTime? CreateTimestamp =>
        liCreateTimestamp == 0 ? null : DateTime.FromFileTimeUtc(liCreateTimestamp);

    public static int Size => Marshal.SizeOf<MibUdp6RowOwnerModule>();
}
