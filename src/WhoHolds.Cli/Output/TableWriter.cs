using WhoHolds.Cli.Extensions;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public static class TableWriter
{
    private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

    public static void WriteFileHolderTable(TextWriter writer, FileHolderProcess[] holders)
    {
        string[] headers =
        [
            "PID",
            "Process Start",
            "Application",
            "Service",
            "Application Type",
            "Statuses",
            "Restartable",
            "Session",
            "Path",
        ];
        bool[] rightAlign = [true, false, false, false, false, false, false, true, false];

        var rows = holders
            .OrderBy(h => h.ProcessId)
            .ThenBy(h => h.StartTime)
            .Select(h =>
                new[]
                {
                    h.ProcessId.ToString(),
                    h.StartTime.ToLocalTime().ToString(_dateFormat),
                    h.ApplicationName ?? "",
                    h.ServiceShortName ?? "",
                    h.ApplicationType,
                    string.Join(", ", h.ApplicationStatus),
                    h.Restartable.ToString(),
                    h.SessionId?.ToString() ?? "",
                    h.ImagePath ?? "",
                }
            )
            .ToArray();

        WriteTable(writer, headers, rightAlign, rows);
    }

    public static void WritePortHolderTable(TextWriter writer, PortHolderProcess[] holders) // TODO: test this
    {
        string[] headers =
        [
            "Local Port",
            "Local IP Address",
            "Owner PID",
            "Owner",
            "Owner Description",
            "Owner Path",
            "Remote IP Address",
            "Remote Port",
            "Created On",
        ];

        bool[] rightAlign = [true, false, false, false, false, false, false, true, false];

        var rows = holders
            .OrderBy(h => h.OwnerProcessId)
            .ThenBy(h => h.LocalPort)
            .Select(h =>
                new[]
                {
                    h.LocalPort.ToString(),
                    h.LocalIPAddress.ToString(),
                    h.OwnerProcessId.ToString(),
                    h.Owner ?? string.Empty,
                    h.OwnerDescription ?? string.Empty,
                    h.OwnerPath ?? string.Empty,
                    h.RemoteIPAddress?.ToString() ?? string.Empty,
                    h.RemotePort?.ToString() ?? string.Empty,
                    h.CreatedOn?.ToString(_dateFormat) ?? string.Empty,
                }
            )
            .ToArray();

        WriteTable(writer, headers, rightAlign, rows);
    }

    private static void WriteTable(
        TextWriter writer,
        string[] headers,
        bool[] rightAlign,
        string[][] rows
    )
    {
        int[] widths = headers.Select((h, i) => rows.Max(r => r[i].Length, h.Length)).ToArray();

        writer.WriteLine(Line(headers));
        writer.WriteLine(Line(widths.Select(w => new string('-', w)).ToArray()));

        foreach (var row in rows)
            writer.WriteLine(Line(row));

        string Line(string[] cells) =>
            string.Join(
                    " ",
                    cells.Select(
                        (c, i) => rightAlign[i] ? c.PadLeft(widths[i]) : c.PadRight(widths[i])
                    )
                )
                .TrimEnd();
    }
}
