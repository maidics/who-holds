using System.Net;
using System.Net.Sockets;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

// marker interface for PortOwnerModuleQuery
internal interface IPortRowOwnerModule // TODO: test implementations
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
