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
    string Owner,
    string OwnerDescription,
    string OwnerPath
)
{
    // TODO: do DescribeFields

    internal static PortHolderProcess FromPortRowOwnerModule(
        IPortRowOwnerModule portOwner,
        string owner,
        string ownerDescription,
        string ownerPath
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
