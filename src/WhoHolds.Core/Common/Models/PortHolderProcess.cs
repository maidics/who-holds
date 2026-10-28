using System.Net;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;

namespace WhoHolds.Core.Common.Models;

public sealed record PortHolderProcess(
    IPAddress LocalIPAddress,
    int LocalPort,
    IPAddress? RemoteIPAddress, // only for TCP; null for UDP
    int? RemotePort, // only for TCP; null for UDP
    int OwnerProcessId,
    DateTime? CreatedOn,
    string? Owner,
    string? OwnerDescription,
    string? OwnerPath
)
{
    public static string DescribeFields()
    {
        return $"Port Holder Process:"
            + $"\n\t- Local IP address: local address the socket is bound to"
            + $"\n\t- Local port: local port the socket is bound to"
            + $"\n\t- Remote IP address: address of the remote endpoint (TCP only, null for UDP)"
            + $"\n\t- Remote port: port of the remote endpoint (TCP only, null for UDP)"
            + $"\n\t- Owner PID: id of the process owning the socket"
            + $"\n\t- Created on: socket creation time (null if unknown)"
            + $"\n\t- Owner: executable file name or service short name of the owner (null if it cannot be resolved)"
            + $"\n\t- Owner description: file description of the owner executable (null if it cannot be resolved)"
            + $"\n\t- Owner path: full path of the owner executable (null if it cannot be resolved)";
    }

    internal static PortHolderProcess FromPortRowOwnerModule(
        IPortRowOwnerModule portOwner,
        string? owner,
        string? ownerDescription,
        string? ownerPath
    )
    {
        return new PortHolderProcess(
            portOwner.LocalAddress,
            portOwner.LocalPort,
            portOwner.RemoteAddress,
            portOwner.RemotePort,
            (int)portOwner.OwnerProcessId,
            portOwner.CreateTimestamp,
            owner,
            ownerDescription,
            ownerPath
        );
    }
}
