# Port Feature

Combinations of list per port number:
- TCP - IPv4
- TCP - IPv6
- UDP - IPv4
- UDP - IPv6

Filtering should be available via an option: `--protocol` (`tcp` | `udp`), `--ipversion`/`-ipv` (`4` | `6`) (these should be ignored for paths)
- Use root command for this and resolve at a lower level (paths and ports are easily distinguishable):
  - `int.Parse`:
    - If succeeds resolve port holders
    - If fails => Try to resolve path:
      - If fails the given parameter is invalid
      - If succeeds continue resolving the holders for the path

## Resolving holders

### 1. `GetExtendedTcpTable` & `GetExtendedUdpTable`

#### Both functions:
- Require a growing buffer mechanism:
  - Modify NativeBuffer to fit `TcpTableClass` `& `UdpTableClass`:
    - Use generic parameter?
    - Maybe ownership of the `NativeBuffer` should be changed so it can only be created via factory methods (e.g. `QueryWithGrowingBuffer`):
      - This way it will only hold what the query was pointing to
- Have a 4 byte header (entry count)

| Function | Table class | Value | What it returns |
| - | - | - | - |
| `GetExtendedTcpTable` | `TCP_TABLE_OWNER_PID_ALL` | 5 | Listening and connected sockets each with its owning PID. |
| `GetExtendedUdpTable` | `UDP_TABLE_OWNER_PID` | 1 | Bound UDP sockets, each with its owning PID. |

Use `AddressFamily` to specify IP version:
- IPv4: `AddressFamily.InterNetwork` (2)
- IPv6: `AddressFamily.InterNetworkV6` (23)