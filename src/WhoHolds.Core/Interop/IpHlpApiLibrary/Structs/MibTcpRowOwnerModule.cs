using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MibTcpRowOwnerModule
    : INativeSized<MibTcpRowOwnerModule>,
        IPortRowOwnerModule
{
    public static AddressFamily AddressFamily => AddressFamily.InterNetwork;
    public static ProtocolType ProtocolType => ProtocolType.Tcp;

    public readonly MibTcpState dwState;
    public readonly uint dwLocalAddr;
    public readonly uint dwLocalPort;
    public readonly uint dwRemoteAddr;
    public readonly uint dwRemotePort;
    public readonly uint dwOwningPid;
    public readonly long liCreateTimestamp;
    public readonly OwningModuleInfoBuffer OwningModuleInfo;

    public ushort LocalPort => BinaryPrimitives.ReverseEndianness((ushort)dwLocalPort);
    public ushort? RemotePort => BinaryPrimitives.ReverseEndianness((ushort)dwRemotePort);
    public uint OwnerProcessId => dwOwningPid;
    public IPAddress LocalAddress => new(dwLocalAddr);
    public IPAddress RemoteAddress => new(dwRemoteAddr);
    ushort? IPortRowOwnerModule.RemotePort => RemotePort;

    public DateTime? CreateTimestamp =>
        liCreateTimestamp == 0 ? null : DateTime.FromFileTimeUtc(liCreateTimestamp);

    public static int Size => Marshal.SizeOf<MibTcpRowOwnerModule>();
}
