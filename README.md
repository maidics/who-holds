# WhoHolds

WhoHolds is a command-line tool for Windows that shows which processes are holding a file open or a network port bound. It's handy when Windows refuses to delete, move, or rename a file because it's "in use by another process," or when a port you need is already taken.

- **Platform:** Windows x64
- **No dependencies:** built with **.NET Native AOT**, _it runs as a single executable with no .NET runtime required_.

## Usage

### Files

```
wh.exe <file-path> [OPTIONS]
```

| Argument | Short | Options    | Default | Type                                                      | Required | Description             |
|----------|-------|------------|---------|-----------------------------------------------------------|----------|-------------------------|
| --format | -f    | text, json | text    | [`OutputFormat`](src/WhoHolds.Cli/Output/OutputFormat.cs) | No       | Sets the output format. |

### Ports

```
wh.exe port [<port>] [OPTIONS]
```

Lists the processes owning TCP and UDP sockets bound to the given local port (0-65535). Omit the port to list every bound port.

| Argument     | Short                      | Options                 | Default | Type                                                              | Required | Description                 |
|--------------|----------------------------|-------------------------|---------|-------------------------------------------------------------------|----------|-----------------------------|
| --protocol   | -p                         | tcp, udp, both          | both    | [`PortProtocol`](src/WhoHolds.Core/Common/Enums/PortProtocol.cs) | No       | Filters by port protocol.   |
| --ip-version | -ip, -ipv, --ipversion     | 4 (ipv4), 6 (ipv6), both | both    | [`IpVersion`](src/WhoHolds.Core/Common/Enums/IpVersion.cs)        | No       | Filters by IP version.      |
| --format     | -f                         | text, json              | text    | [`OutputFormat`](src/WhoHolds.Cli/Output/OutputFormat.cs)         | No       | Sets the output format.     |

> Note: aborting tests or the root command can lead to orphaned Restart Manager sessions.

**Examples:**

```
wh.exe port 8080
wh.exe port 53 -p udp -ipv 6
wh.exe port -f json
```

> **Note:** services hosted in `svchost.exe` report their service name (e.g. `RpcEptMapper`) as owner path instead of an executable path.

## Exit codes

| Code | Description                          |
|------|--------------------------------------|
| 0    | Specified object has **no holders**. |
| 1    | Specified object has **holders**.    |
| 2    | Operation failed.                    |

For files, holders are all processes that have the file open, regardless of the share mode they opened it with: a holder does not necessarily block deleting, moving or renaming the file.

For ports, holders are all processes owning a socket bound to the port: listening and connected TCP sockets as well as bound UDP sockets.

## JSON Output

When run with JSON output, every command returns an object with the same shape:

| Field       | Type                  | Description                                                                                                                                                                |
|-------------|-----------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `succeeded` | `boolean`             | `true` if the operation completed successfully, otherwise `false`.                                                                                                         |
| `value`     | *see below* \| `null` | The result of the operation. Always `null` when `succeeded` is `false`.                                                                                                    |
| `errors`    | `string[]`            | Errors that occurred during the operation. May contain non-fatal failures on success (e.g. image path resolution failure: reused process ID, access privilege error etc.). |

#### Value types by command

| Command            | Value Type                                                                    | Note                                                                                                                                                       |
|--------------------|-------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `wh <file-path>`   | [`FileHolderProcess[]`](src/WhoHolds.Core/Common/Models/FileHolderProcess.cs) | `null` value on FileHolderProcess.ImagePath means the path resolution failed. You may find an error message regarding this inside Result.Errors.          |
| `wh port [<port>]` | [`PortHolderProcess[]`](src/WhoHolds.Core/Common/Models/PortHolderProcess.cs) | `remoteIPAddress` and `remotePort` are `null` for UDP sockets. `null` owner fields mean the owner could not be resolved; see Result.Errors for the reason. |

**File example:**

```json
{
  "succeeded": true,
  "value": [
    {
      "processId": 17260,
      "startTime": "2026-10-22T12:17:53.4362271Z",
      "applicationName": "wh",
      "serviceShortName": null,
      "applicationType": "Critical",
      "applicationStatus": [
        "Running"
      ],
      "imagePath": "C:\\Users\\mmaidics\\Downloads\\who-holds-win-x64\\wh.exe",
      "sessionId": 1,
      "restartable": false
    }
  ],
  "errors": []
}
```

**Port example:**

```json
{
  "succeeded": true,
  "value": [
    {
      "localIPAddress": "127.0.0.1",
      "localPort": 8080,
      "remoteIPAddress": "0.0.0.0",
      "remotePort": 0,
      "ownerProcessId": 21468,
      "createdOn": "2026-10-28T09:12:45.4362271Z",
      "owner": "node.exe",
      "ownerDescription": "Node.js JavaScript Runtime",
      "ownerPath": "C:\\Program Files\\nodejs\\node.exe"
    }
  ],
  "errors": []
}
```