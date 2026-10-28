using System.Net;
using System.Net.Sockets;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

// marker interface for PortOwnerModuleTableQuery
internal interface IPortRowOwnerModule
{
    static abstract AddressFamily AddressFamily { get; }
    static abstract ProtocolType ProtocolType { get; }

    IPAddress LocalAddress { get; }
    ushort LocalPort { get; }
    IPAddress? RemoteAddress { get; }
    ushort? RemotePort { get; }
    uint OwnerProcessId { get; }
    DateTime? CreateTimestamp { get; }
}
