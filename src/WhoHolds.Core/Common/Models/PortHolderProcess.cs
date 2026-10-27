using System.Net;

namespace WhoHolds.Core.Common.Models;

public sealed record PortHolderProcess(
    IPAddress LocalIPAddress,
    int LocalPort,
    IPAddress? RemoteIPAddress, // only for TCP; null for UDP
    int? RemotePort, // only for TCP; null for UDP
    int OwnerProcessId,
    DateTime CreatedOn,
    string Owner,
    string OwnerDescription,
    string OwnerPath
);
