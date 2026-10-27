using System.Net;
using WhoHolds.Core.Common.Enums;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Interop.Services;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core;

public static class WhoHolds
{
    public static Result<FileHolderProcess[]> File(string file)
    {
        var resolutionResult = PathUtils.ResolveFullPath(file);

        if (!resolutionResult.Succeeded)
            return Result.Failure(resolutionResult.Errors);

        using var session = new RestartManagerSession([resolutionResult.Value]);
        var startResult = session.Start();

        if (!startResult.Succeeded)
            return Result.Failure(startResult.Errors);

        var getResult = session.GetProcesses(out _);

        if (!getResult.Succeeded)
            return Result.Failure(getResult.Errors);

        List<string> pathErrors = [];

        var processes = getResult
            .Value.Select(p =>
            {
                var pathResult = ProcessPathResolver.Resolve(p);

                if (pathResult.Succeeded)
                {
                    return p.ToHolderProcess(pathResult.Value);
                }

                pathErrors.AddRange(pathResult.Errors);
                return p.ToHolderProcess(null);
            })
            .ToArray();

        return Result.Success(processes, pathErrors.ToArray());
    }

    // 16 bits; 0 - 65535
    public static Result<PortHolderProcess[]> Port(
        int? port, // null is all
        IpVersion ipVersion,
        PortProtocol portProtocol
    )
    {
        if (port is < IPEndPoint.MinPort or > IPEndPoint.MaxPort)
            return Result.Failure(
                $"Port must be between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort} (inclusive)."
            );

        var tcp = portProtocol is PortProtocol.Tcp or PortProtocol.Both;
        var udp = portProtocol is PortProtocol.Udp or PortProtocol.Both;
        var v4 = ipVersion is IpVersion.IPv4 or IpVersion.Both;
        var v6 = ipVersion is IpVersion.IPv6 or IpVersion.Both;

        var collector = new PortHolderCollector(port);

        if (tcp && v4)
            collector.Collect<MibTcpRowOwnerModule>();
        if (tcp && v6)
            collector.Collect<MibTcp6RowOwnerModule>();
        if (udp && v4)
            collector.Collect<MibUdpRowOwnerModule>();
        if (udp && v6)
            collector.Collect<MibUdp6RowOwnerModule>();

        return collector.ToResult();
    }
}
