using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MibUdp6RowOwnerModule : IPortRowOwnerModule
{
    public static AddressFamily AddressFamily => AddressFamily.InterNetworkV6;
    public static ProtocolType ProtocolType => ProtocolType.Udp;

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
    public IPAddress? RemoteAddress => null;
    public ushort? RemotePort => null;
    public uint OwnerProcessId => dwOwningPid;
    public IPAddress LocalAddress => new(ucLocalAddr, dwLocalScopeId);
    public bool SpecificPortBind => (dwFlags & 1) != 0;
    public DateTime? CreateTimestamp =>
        liCreateTimestamp == 0 ? null : DateTime.FromFileTimeUtc(liCreateTimestamp);
}
