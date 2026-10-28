using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MibTcp6RowOwnerModule : IPortRowOwnerModule
{
    public static AddressFamily AddressFamily => AddressFamily.InterNetworkV6;
    public static ProtocolType ProtocolType => ProtocolType.Tcp;

    public readonly IPv6AddressBuffer ucLocalAddr;
    public readonly uint dwLocalScopeId;
    public readonly uint dwLocalPort;
    public readonly IPv6AddressBuffer ucRemoteAddr;
    public readonly uint dwRemoteScopeId;
    public readonly uint dwRemotePort;
    public readonly MibTcpState dwState; // near the end here, first in the IPv4 row
    public readonly uint dwOwningPid;
    public readonly long liCreateTimestamp;
    public readonly OwningModuleInfoBuffer OwningModuleInfo;

    public ushort LocalPort => BinaryPrimitives.ReverseEndianness((ushort)dwLocalPort);
    public ushort? RemotePort => BinaryPrimitives.ReverseEndianness((ushort)dwRemotePort);
    public uint OwnerProcessId => dwOwningPid;
    public IPAddress LocalAddress => new(ucLocalAddr, dwLocalScopeId);
    public IPAddress RemoteAddress => new(ucRemoteAddr, dwRemoteScopeId);
    public DateTime? CreateTimestamp =>
        liCreateTimestamp == 0 ? null : DateTime.FromFileTimeUtc(liCreateTimestamp);
}
