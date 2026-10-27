using System.Buffers.Binary;
using System.Net;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MibUdpRowOwnerModule
    : INativeSized<MibUdpRowOwnerModule>,
        IPortRowOwnerModule
{
    public readonly uint dwLocalAddr;
    public readonly uint dwLocalPort;
    public readonly uint dwOwningPid;

    // 4 bytes padding: liCreateTimestamp is 8-byte aligned
    public readonly long liCreateTimestamp;
    public readonly int dwFlags; // union { int SpecificPortBind : 1; int dwFlags; }

    // 4 bytes padding: OwningModuleInfo is 8-byte aligned
    public readonly OwningModuleInfoBuffer OwningModuleInfo;

    public ushort LocalPort => BinaryPrimitives.ReverseEndianness((ushort)dwLocalPort);
    public IPAddress LocalAddress => new(dwLocalAddr);
    public bool SpecificPortBind => (dwFlags & 1) != 0;
    public DateTime? CreateTimestamp =>
        liCreateTimestamp == 0 ? null : DateTime.FromFileTimeUtc(liCreateTimestamp);

    public static int Size => Marshal.SizeOf<MibUdpRowOwnerModule>();
}
