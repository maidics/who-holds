using System.Net;
using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Interop.Services;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Services;

internal sealed class PortOwnerModuleInfoQueryTests
{
    private static Socket BindOnLoopback(AddressFamily addressFamily, ProtocolType protocolType)
    {
        var socketType = protocolType == ProtocolType.Udp ? SocketType.Dgram : SocketType.Stream;
        var socket = new Socket(addressFamily, socketType, protocolType);
        try
        {
            var loopback =
                addressFamily == AddressFamily.InterNetworkV6
                    ? IPAddress.IPv6Loopback
                    : IPAddress.Loopback;

            socket.Bind(new IPEndPoint(loopback, 0));

            if (socketType == SocketType.Stream)
                socket.Listen();

            return socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    [Test]
    [GenerateGenericTest(typeof(MibTcpRowOwnerModule))]
    [GenerateGenericTest(typeof(MibTcp6RowOwnerModule))]
    [GenerateGenericTest(typeof(MibUdpRowOwnerModule))]
    [GenerateGenericTest(typeof(MibUdp6RowOwnerModule))]
    public void ShouldResolveOwnProcessAsOwnerModule<TRow>()
        where TRow : unmanaged, IPortRowOwnerModule
    {
        using var socket = BindOnLoopback(TRow.AddressFamily, TRow.ProtocolType);
        var endPoint = (IPEndPoint)socket.LocalEndPoint!;

        var tableResult = PortOwnerModuleTableQuery.Query<TRow>();
        tableResult.ShouldBeResultedTo(true);

        var entry = tableResult
            .Value.Entries.Where(e =>
                e.LocalPort == endPoint.Port && e.LocalAddress.Equals(endPoint.Address)
            )
            .ShouldHaveSingleItem();

        var result = PortOwnerModuleInfoQuery.Query(entry);
        result.ShouldBeResultedTo(true);

        var processPath = Environment.ProcessPath.ShouldNotBeNull();
        result.Value.modulePath.ShouldBe(processPath, StringCompareShould.IgnoreCase);
        result.Value.module.ShouldBe(Path.GetFileName(processPath), StringCompareShould.IgnoreCase);
    }

    [Test]
    [GenerateGenericTest(typeof(MibTcpRowOwnerModule))]
    [GenerateGenericTest(typeof(MibTcp6RowOwnerModule))]
    [GenerateGenericTest(typeof(MibUdpRowOwnerModule))]
    [GenerateGenericTest(typeof(MibUdp6RowOwnerModule))]
    public void ShouldFailForEntryWithoutOwner<TRow>()
        where TRow : unmanaged, IPortRowOwnerModule
    {
        var result = PortOwnerModuleInfoQuery.Query(default(TRow)); // PID 0: no owner to resolve

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
    }
}
