# Port Feature

Combinations of list per port number:
- TCP - IPv4
- TCP - IPv6
- UDP - IPv4
- UDP - IPv6

The [`port`](../src/WhoHolds.Cli/Commands/GetPortHoldersCommand.cs) subcommand takes an optional port argument (omit to list all ports) and filters via options: `--protocol`/`-p` (`tcp` | `udp` | `both`), `--ip-version`/`-ip`/`-ipv`/`--ipversion` (`4` | `6` | `both`). Usage is documented in the [README](../README.md#ports).

[`WhoHolds.Port(port, ipVersion, portProtocol)`](../src/WhoHolds.Core/WhoHolds.cs) validates the port range (`IPEndPoint.MinPort`-`IPEndPoint.MaxPort`) and queries one table per selected combination. Two enums select the combinations:
- [`PortProtocol`](../src/WhoHolds.Core/Common/Enums/PortProtocol.cs): `Tcp`, `Udp`, `Both` (default)
- [`IpVersion`](../src/WhoHolds.Core/Common/Enums/IpVersion.cs): `IPv4`, `IPv6`, `Both` (default)

## Resolving holders

[`PortHolderCollector`](../src/WhoHolds.Core/Interop/Services/PortHolderCollector.cs) collects the rows of each table, keeps the ones whose local port matches and resolves their owner module. The result fails only if every table query failed; otherwise failures are reported in `Result.Errors`.

### 1. `GetExtendedTcpTable` & `GetExtendedUdpTable`

[`PortOwnerModuleTableQuery`](../src/WhoHolds.Core/Interop/Services/PortOwnerModuleTableQuery.cs):
- Starts with a 64 KB buffer and grows it on `ERROR_INSUFFICIENT_BUFFER` (122) to the returned `pdwSize` + 25% headroom, capped at 5 attempts (guards against something opening and closing sockets non-stop).
- The table header is the entry count (4 bytes) padded to 8 bytes; rows follow it.

| Function | Table class | Value | What it returns |
| - | - | - | - |
| `GetExtendedTcpTable` | `TCP_TABLE_OWNER_MODULE_ALL` | 8 | Listening and connected sockets each with its owning PID. |
| `GetExtendedUdpTable` | `UDP_TABLE_OWNER_MODULE` | 2 | Bound UDP sockets, each with its owning PID. |

```c++
// MIB_TCPROW_OWNER_MODULE (IPv4), same idea for MIB_TCP6ROW_OWNER_MODULE
DWORD         dwState;
DWORD         dwLocalAddr;
DWORD         dwLocalPort;
DWORD         dwRemoteAddr;
DWORD         dwRemotePort;
DWORD         dwOwningPid;
LARGE_INTEGER liCreateTimestamp;     // when the socket was created; 0 if unknown
ULONGLONG     OwningModuleInfo[16];  // opaque, for GetOwnerModuleFromTcpEntry
```

`AddressFamily` specifies the IP version:
- IPv4: `AddressFamily.InterNetwork` (2)
- IPv6: `AddressFamily.InterNetworkV6` (23)

Each row maps to one [`PortHolderProcess`](../src/WhoHolds.Core/Common/Models/PortHolderProcess.cs): rows are not grouped by PID, so a process owning both the IPv4 and IPv6 socket is listed twice.

### 2. `GetOwnerModuleFromTcpEntry` & `GetOwnerModuleFromUdpEntry`

[`PortOwnerModuleInfoQuery`](../src/WhoHolds.Core/Interop/Services/PortOwnerModuleInfoQuery.cs) queries `TCPIP_OWNER_MODULE_BASIC_INFO` (sized on `ERROR_INSUFFICIENT_BUFFER`, one retry) to get information on who owns the socket:

| Owner | Module name | Module path |
| - | - | - |
| A normal program | The executable's file name (e.g. node.exe). | The full executable path (e.g. `C:\Program Files\nodejs\node.exe`). |
| A service inside a shared `svchost.exe` | The service short name (e.g. `Dnscache`). | The service short name as well (e.g. `RpcEptMapper`), not a path. |

This returns technical names (unlike `Restart Manager`):
- Program names: resolved via `FileVersionInfo.GetVersionInfo(path).FileDescription` into `OwnerDescription`, only when the module path is fully qualified.
- Failures (e.g. processes that you're not allowed to inspect) are reported in `Result.Errors` and the owner fields stay `null`.

## Not implemented

- Dropping rows owned by PID 0 (System Idle Process).
- Grouping rows by PID.
- Service display names via `GetServiceDisplayNameW` (Service Control Manager).
- Special casing PID 4 (System) to skip the owner module lookup.
