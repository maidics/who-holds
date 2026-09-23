# WhoHolds

WhoHolds is a command-line tool for Windows that shows which processes are holding a file open. It's handy when Windows refuses to delete, move, or rename a file because it's "in use by another process."

- **Platform:** Windows x64
- **No dependencies:** built with **.NET Native AOT**, _it runs as a single executable with no .NET runtime required_.

> **Note:** WhoHolds is a work in progress. More features are planned.

## Usage

```
wh.exe <absolute-file-path> [OPTIONS]
```

## Arguments

| Argument | Short | Options    | Default | Type                                                      | Required | Description             |
|----------|-------|------------|---------|-----------------------------------------------------------|----------|-------------------------|
| --format | -f    | text, json | text    | [`OutputFormat`](src/WhoHolds.Cli/Output/OutputFormat.cs) | No       | Sets the output format. |

## Exit codes

| Code | Description                         |
|------|-------------------------------------|
| 0    | Specified object is **not locked**. |
| 1    | Specified object is **locked**.     |
| 2    | Operation failed.                   |

## JSON Output

When run with JSON output, every command returns an object with the same shape:

| Field       | Type                  | Description                                                                         |
|-------------|-----------------------|-------------------------------------------------------------------------------------|
| `succeeded` | `boolean`             | `true` if the operation completed successfully, otherwise `false`.                  |
| `value`     | *see below* \| `null` | The result of the operation. Always `null` when `succeeded` is `false`.             |
| `errors`    | `string[]`            | Errors that occurred during the operation. Always empty when `succeeded` is `true`. |

#### Value types by command

| Command                    | Value Type                                                            |
|----------------------------|-----------------------------------------------------------------------|
| `wh <absolute-file-path> ` | [`HolderProcess[]`](src/WhoHolds.Core/Common/Models/HolderProcess.cs) |

**Example:**

```json
{
  "succeeded": true,
  "value": [
    {
      "processId": 16364,
      "startTime": "2026-09-23T06:49:40.240106Z",
      "applicationName": "Microsoft Excel",
      "serviceShortName": null,
      "applicationType": "MainWindow",
      "applicationStatus": [
        "Running"
      ],
      "sessionId": 1,
      "restartable": true
    }
  ],
  "errors": []
}
```