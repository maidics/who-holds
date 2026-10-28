using System.Net;
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

    private static readonly PortHolderProcess _portHolder = new(
        IPAddress.Loopback,
        1,
        null,
        null,
        1,
        null,
        null,
        null,
        null
    );

    private static string[] PortLines(PortHolderProcess[] holders)
    {
        var output = new StringWriter();
        TableWriter.WritePortHolderTable(output, holders);

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

    [Test]
    public void ShouldWriteOnlyPortHolderHeaderAndSeparatorWhenNoHolders()
    {
        PortLines([])
            .ShouldBe([
                "Local Port Local IP Address Owner PID Owner Owner Description Owner Path Remote IP Address Remote Port Created On",
                "---------- ---------------- --------- ----- ----------------- ---------- ----------------- ----------- ----------",
            ]);
    }

    [Test]
    public void ShouldWriteAlignedPortHolderTableSortedByOwnerProcessId()
    {
        var rpc = _portHolder with
        {
            LocalIPAddress = IPAddress.Any,
            LocalPort = 135,
            OwnerProcessId = 1200,
            Owner = "RpcEptMapper",
            OwnerPath = "RpcEptMapper",
        };
        var node = _portHolder with
        {
            LocalPort = 5000,
            RemoteIPAddress = IPAddress.Loopback,
            RemotePort = 53000,
            OwnerProcessId = 42,
            Owner = "node.exe",
            OwnerDescription = "Node.js JavaScript Runtime",
            OwnerPath = @"C:\Program Files\nodejs\node.exe",
        };

        PortLines([rpc, node])
            .ShouldBe([
                "Local Port Local IP Address Owner PID Owner        Owner Description          Owner Path                       Remote IP Address Remote Port Created On",
                "---------- ---------------- --------- ------------ -------------------------- -------------------------------- ----------------- ----------- ----------",
                "      5000 127.0.0.1        42        node.exe     Node.js JavaScript Runtime C:\\Program Files\\nodejs\\node.exe 127.0.0.1               53000",
                "       135 0.0.0.0          1200      RpcEptMapper                            RpcEptMapper",
            ]);
    }

    [Test]
    public void ShouldSortByLocalPortWhenOwnerProcessIdsAreEqual()
    {
        var higher = _portHolder with { LocalPort = 8080 };
        var lower = _portHolder with { LocalPort = 80 };

        var rows = PortLines([higher, lower])[2..];

        rows.Length.ShouldBe(2);
        rows[0].TrimStart().ShouldStartWith("80 ");
        rows[1].TrimStart().ShouldStartWith("8080 ");
    }

    [Test]
    public void ShouldWriteIPv6Addresses()
    {
        var row = PortLines([
            _portHolder with
            {
                LocalIPAddress = IPAddress.IPv6Loopback,
                RemoteIPAddress = IPAddress.IPv6Any,
                RemotePort = 2,
            },
        ])[2];

        row.ShouldContain(" ::1 ");
        row.ShouldContain(" :: ");
    }

    [Test]
    public void ShouldNotWritePortHolderTrailingWhitespace()
    {
        var lines = PortLines([
            _portHolder,
            _portHolder with
            {
                OwnerProcessId = 2,
                Owner = "owner",
                OwnerPath = "path",
            },
        ]);

        lines.ShouldAllBe(line => line == line.TrimEnd());
    }
}
