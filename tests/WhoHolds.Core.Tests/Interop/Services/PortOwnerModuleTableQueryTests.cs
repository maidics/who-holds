using System.Net;
using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Interop.Services;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Services;

internal sealed class PortOwnerModuleTableQueryTests
{
    private static Socket BindOnLoopback(AddressFamily addressFamily, ProtocolType protocolType)
    {
        var socketType = protocolType == ProtocolType.Udp ? SocketType.Dgram : SocketType.Stream; // UDP with Stream is not a valid combination for Socket ctor
        var socket = new Socket(addressFamily, socketType, protocolType);
        try
        {
            var loopback =
                addressFamily == AddressFamily.InterNetworkV6
                    ? IPAddress.IPv6Loopback
                    : IPAddress.Loopback;

            socket.Bind(new IPEndPoint(loopback, 0)); // 0 means the OS picks the port

            if (socketType == SocketType.Stream)
                socket.Listen(); // bound-only TCP sockets don't appear in the TCP table; UDP shows once bound

            return socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    [Test]
    public void ShouldQueryTcpOwnerModules()
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetwork, ProtocolType.Tcp);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!; // the actual port, not 0

        var result = PortOwnerModuleTableQuery.Query<MibTcpRowOwnerModule>();
        result.ShouldBeResultedTo(true);

        var entry = result
            .Value.Entries.Where(e =>
                e.LocalPort == endPoint.Port && e.LocalAddress.Equals(endPoint.Address)
            )
            .ShouldHaveSingleItem();

        entry.dwOwningPid.ShouldBe((uint)Environment.ProcessId);
        entry.dwState.ShouldBe(MibTcpState.Listen); // MIB_TCP_STATE_LISTEN = 2
    }

    [Test]
    public void ShouldQueryTcp6OwnerModules()
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetworkV6, ProtocolType.Tcp);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!; // the actual port, not 0

        var result = PortOwnerModuleTableQuery.Query<MibTcp6RowOwnerModule>();
        result.ShouldBeResultedTo(true);

        var entry = result
            .Value.Entries.Where(e =>
                e.LocalPort == endPoint.Port && e.LocalAddress.Equals(endPoint.Address)
            )
            .ShouldHaveSingleItem();

        entry.dwOwningPid.ShouldBe((uint)Environment.ProcessId);
        entry.dwState.ShouldBe(MibTcpState.Listen); // MIB_TCP_STATE_LISTEN = 2
    }

    [Test]
    public void ShouldQueryUdpOwnerModules()
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetwork, ProtocolType.Udp);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!; // the actual port, not 0

        var result = PortOwnerModuleTableQuery.Query<MibUdpRowOwnerModule>();
        result.ShouldBeResultedTo(true);

        var entry = result
            .Value.Entries.Where(e =>
                e.LocalPort == endPoint.Port && e.LocalAddress.Equals(endPoint.Address)
            )
            .ShouldHaveSingleItem();

        entry.dwOwningPid.ShouldBe((uint)Environment.ProcessId);
    }

    [Test]
    public void ShouldQueryUdp6OwnerModules()
    {
        using var socket = BindOnLoopback(AddressFamily.InterNetworkV6, ProtocolType.Udp);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!; // the actual port, not 0

        var result = PortOwnerModuleTableQuery.Query<MibUdp6RowOwnerModule>();
        result.ShouldBeResultedTo(true);

        var entry = result
            .Value.Entries.Where(e =>
                e.LocalPort == endPoint.Port && e.LocalAddress.Equals(endPoint.Address)
            )
            .ShouldHaveSingleItem();

        entry.dwOwningPid.ShouldBe((uint)Environment.ProcessId);
    }
}
