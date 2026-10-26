using WhoHolds.Cli.Extensions;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public static class TableWriter
{
    public static void Write(TextWriter writer, HolderProcess[] holders) // TODO: add testing
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
            "Path",
        ];
        bool[] rightAlign = [true, false, false, false, false, false, false, false];

        var rows = holders
            .OrderBy(h => h.ProcessId)
            .ThenBy(h => h.StartTime)
            .Select(h =>
                new[]
                {
                    h.ProcessId.ToString(),
                    h.StartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    h.ApplicationName ?? "",
                    h.ServiceShortName ?? "",
                    h.ApplicationType,
                    string.Join(", ", h.ApplicationStatus),
                    h.Restartable.ToString(),
                    h.ImagePath ?? "",
                }
            )
            .ToArray();

        int[] widths = headers.Select((h, i) => rows.Max(r => r[i].Length, h.Length)).ToArray();

        string Line(string[] cells) =>
            string.Join(
                    " ",
                    cells.Select(
                        (c, i) => rightAlign[i] ? c.PadLeft(widths[i]) : c.PadRight(widths[i])
                    )
                )
                .TrimEnd();

        writer.WriteLine(Line(headers));
        writer.WriteLine(Line(widths.Select(w => new string('-', w)).ToArray()));

        foreach (var row in rows)
            writer.WriteLine(Line(row));
    }
}
