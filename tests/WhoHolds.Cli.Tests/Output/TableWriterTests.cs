using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class TableWriterTests
{
    private static readonly DateTime _startTime = new(2026, 10, 26, 9, 57, 38, DateTimeKind.Utc);

    private static readonly FileHolderProcess _holder = new(
        1,
        _startTime,
        null,
        null,
        string.Empty,
        [],
        null,
        null,
        false
    );

    private static string[] Lines(FileHolderProcess[] holders)
    {
        var output = new StringWriter();
        TableWriter.WriteFileHolderTable(output, holders);

        return output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string Local(DateTime utc) => utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

    [Test]
    public void ShouldWriteOnlyHeaderAndSeparatorWhenNoHolders()
    {
        Lines([])
            .ShouldBe([
                "PID Process Start Application Service Application Type Statuses Restartable Session Path",
                "--- ------------- ----------- ------- ---------------- -------- ----------- ------- ----",
            ]);
    }

    [Test]
    public void ShouldWriteAlignedTableSortedByProcessId()
    {
        var notepadStart = _startTime.AddMinutes(5);

        var system = _holder with
        {
            ProcessId = 4,
            ApplicationName = "System",
            ApplicationType = "Critical",
            ApplicationStatus = ["Running"],
        };
        var notepad = _holder with
        {
            ProcessId = 1234,
            StartTime = notepadStart,
            ApplicationName = "Notepad",
            ServiceShortName = "svc",
            ApplicationType = "MainWindow",
            ApplicationStatus = ["Running", "Restarted"],
            ImagePath = @"C:\Windows\notepad.exe",
            SessionId = 1,
            Restartable = true,
        };

        Lines([notepad, system])
            .ShouldBe([
                " PID Process Start       Application Service Application Type Statuses           Restartable Session Path",
                "---- ------------------- ----------- ------- ---------------- ------------------ ----------- ------- ----------------------",
                $"   4 {Local(_startTime)} System              Critical         Running            False",
                $"1234 {Local(notepadStart)} Notepad     svc     MainWindow       Running, Restarted True              1 C:\\Windows\\notepad.exe",
            ]);
    }

    [Test]
    public void ShouldSortByStartTimeWhenProcessIdsAreEqual()
    {
        var earlier = _holder with { ApplicationName = "earlier" };
        var later = _holder with
        {
            StartTime = _startTime.AddSeconds(1),
            ApplicationName = "later",
        };

        var rows = Lines([later, earlier])[2..];

        rows.Length.ShouldBe(2);
        rows[0].ShouldContain("earlier");
        rows[1].ShouldContain("later");
    }

    [Test]
    public void ShouldWriteStartTimeAsLocalTime()
    {
        Lines([_holder])[2].ShouldContain(Local(_startTime));
    }

    [Test]
    [Arguments(true, "True")]
    [Arguments(false, "False")]
    public void ShouldWriteRestartable(bool restartable, string expected)
    {
        Lines([_holder with { Restartable = restartable }])[2].ShouldEndWith(expected);
    }

    [Test]
    public void ShouldNotWriteTrailingWhitespace()
    {
        var lines = Lines([
            _holder with
            {
                ImagePath = null,
            },
            _holder with
            {
                ProcessId = 2,
                ImagePath = "path",
            },
        ]);

        lines.ShouldAllBe(line => line == line.TrimEnd());
    }
}
