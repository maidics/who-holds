# Port Feature

Combinations of list per port number:
- TCP - IPv4
- TCP - IPv6
- UDP - IPv4
- UDP - IPv6

Filtering should be available via an option: `--protocol` (`tcp` | `udp`), `--ipversion`/`-ipv` (`4` | `6`)
- Add an additional command for ports (`--port`/`-p`)
  - `int.Parse`:
    - If succeeds resolve port holders
    - If fails => Try to resolve path:
      - If fails the given parameter is invalid
      - If succeeds continue resolving the holders for the path

## Resolving holders

### 1. `GetExtendedTcpTable` & `GetExtendedUdpTable`

#### Both functions:
- Pass 0 as size first =(on `ERROR_INSUFFICIENT_BUFFER` - 122)=> it returns the required `pdwSize`:
  - Should use growing buffer with capped retry count (guard against something opening and closing sockets non-stop)
- Have a 4 byte header (entry count)

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
LARGE_INTEGER liCreateTimestamp;     // when the socket was created; confirm whether PID was reused or not
ULONGLONG     OwningModuleInfo[16];  // opaque, for GetOwnerModuleFromTcpEntry
```

Use `AddressFamily` to specify IP version:
- IPv4: `AddressFamily.InterNetwork` (2)
- IPv6: `AddressFamily.InterNetworkV6` (23)

`PortHolderQueryOptions`
- We need two enums: 
  - `PortProtocol`: `Tcp`, `Udp`, `Both` (default)
  - `IpVersion`: `IPv4`, `IPv6`, `Both` (default)

Filter and group by PID (according to passed options):
- Filter local ports (ports bound on this machine).
- Drop rows that belong to no one. Windows reports idle ports with 0 PID (System Idle Process).
- Collect ports via PID, because one process can own multiple (e.g. owns the IPv4 and IPv6 port as well).

### 2. `GetOwnerModuleFromTcpEntry` & `GetOwnerModuleFromUdpEntry`

Returns information on who owns the socket:

| Owner | Module name | Module path |
| - | - | - |
| A normal program | The executable's file name (e.g. node.exe). | The full executable path (e.g. `C:\Program Files\nodejs\node.exe`). |
| A service inside a shared `svchost.exe` | The service short name (e.g. `Dnscache`). | This may be the path to `svchost.exe` - test this. |

This returns technical names (unlike `Restart Manager`):
- Program names: resolve via `FileVersionInfo.GetVersionInfo(path).FileDescription`.
- Service names: resolve via `GetServiceDisplayNameW` (Service Control Manager).
- This may fail for processes that you're not allowed to inspect.
- Treat errors as name unknown.
- Special case: PID 4 - report as System with no start time, do not call the process lookup at all - this will fail for admin users as well.