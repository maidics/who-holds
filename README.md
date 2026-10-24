# WhoHolds

WhoHolds is a command-line tool for Windows that shows which processes are holding a file open. It's handy when Windows refuses to delete, move, or rename a file because it's "in use by another process."

- **Platform:** Windows x64
- **No dependencies:** built with **.NET Native AOT**, _it runs as a single executable with no .NET runtime required_.

> **Note:** WhoHolds is a work in progress. More features are planned.

## Usage

```
wh.exe <file-path> [OPTIONS]
```

## Arguments

| Argument | Short | Options    | Default | Type                                                      | Required | Description             |
|----------|-------|------------|---------|-----------------------------------------------------------|----------|-------------------------|
| --format | -f    | text, json | text    | [`OutputFormat`](src/WhoHolds.Cli/Output/OutputFormat.cs) | No       | Sets the output format. |

## Exit codes

| Code | Description                          |
|------|--------------------------------------|
| 0    | Specified object has **no holders**. |
| 1    | Specified object has **holders**.    |
| 2    | Operation failed.                    |

Holders are all processes that have the object open, regardless of the share mode they opened it with: a holder does not necessarily block deleting, moving or renaming the object.

## JSON Output

When run with JSON output, every command returns an object with the same shape:

| Field       | Type                  | Description                                                                                                                                                                |
|-------------|-----------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `succeeded` | `boolean`             | `true` if the operation completed successfully, otherwise `false`.                                                                                                         |
| `value`     | *see below* \| `null` | The result of the operation. Always `null` when `succeeded` is `false`.                                                                                                    |
| `errors`    | `string[]`            | Errors that occurred during the operation. May contain non-fatal failures on success (e.g. image path resolution failure: reused process ID, access privilege error etc.). |

#### Value types by command

| Command           | Value Type                                                            | Note                                                                                                                                         |
|-------------------|-----------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| `wh <file-path> ` | [`HolderProcess[]`](src/WhoHolds.Core/Common/Models/HolderProcess.cs) | `null` value on HolderProcess.ImagePath means the path resolution failed. You may find an error message ragarding this inside Result.Errors. |

**Example:**

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