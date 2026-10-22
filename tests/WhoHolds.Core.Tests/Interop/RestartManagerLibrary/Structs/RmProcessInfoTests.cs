using WhoHolds.Core.Interop.RestartManagerLibrary.Enums;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.RestartManagerLibrary.Structs;

[InheritsTests]
internal sealed class RmProcessInfoTests() : NativeStructTestBase<RmProcessInfo>(668)
{
    [Test]
    [Arguments("", null, "", null)]
    [Arguments("\0", null, "", null)]
    [Arguments("app-name", "app-name", "service-name", "service-name")]
    public void InlineCharArrayHelpersShouldReturnCorrectValue(
        string appName,
        string? expectedAppName,
        string serviceShortName,
        string? expectedServiceShortName
    )
    {
        var processInfo = new RmProcessInfo(
            default,
            appName,
            serviceShortName,
            default,
            default,
            default,
            default
        );

        ShouldBeStringTestExtensions.ShouldBe(processInfo.AppName, expectedAppName);
        ShouldBeStringTestExtensions.ShouldBe(
            processInfo.ServiceShortName,
            expectedServiceShortName
        );
    }

    [Test]
    [Arguments(-1, true)]
    [Arguments(0, false)]
    [Arguments(1, true)]
    [Arguments(2, true)]
    public void IsRestartAbleShouldReturnCorrectValue(int restartable, bool expected)
    {
        var processInfo = new RmProcessInfo(
            default,
            string.Empty,
            default,
            default,
            default,
            default,
            restartable
        );

        ShouldBeTestExtensions.ShouldBe(processInfo.IsRestartable, expected);
    }

    [Test]
    [Arguments(uint.MaxValue, null)]
    [Arguments(999, 999)]
    public void SessionIdShouldReturnCorrectValue(uint sessionId, int? expected)
    {
        var processInfo = new RmProcessInfo(
            default,
            string.Empty,
            default,
            default,
            default,
            sessionId,
            default
        );

        ShouldBeTestExtensions.ShouldBe(processInfo.SessionId, expected);
    }

    [Test]
    [Arguments(null)]
    [Arguments("C:\\wh.exe")]
    public void ToHolderProcessShouldMapCorrectly(string? path)
    {
        var processInfo = new RmProcessInfo(
            new RmUniqueProcess(),
            "test-app",
            null,
            RmAppType.Console,
            RmAppStatus.StatusRunning,
            11,
            1
        );

        var holder = processInfo.ToHolderProcess(path);
        ShouldBeTestExtensions.ShouldBe(holder.ProcessId, (int)processInfo.Process.dwProcessId);
        holder.StartTime.ShouldBe(processInfo.Process.ProcessStartTime.ToDateTimeUtc());
        ShouldBeStringTestExtensions.ShouldBe(holder.ApplicationName, processInfo.AppName);
        ShouldBeStringTestExtensions.ShouldBe(
            holder.ServiceShortName,
            processInfo.ServiceShortName
        );
        ShouldBeStringTestExtensions.ShouldBe(
            holder.ApplicationType,
            processInfo.ApplicationType.ToString()
        );
        ShouldBeTestExtensions.ShouldBe<List<string>>(holder.ApplicationStatus, ["Running"]);
        ShouldBeStringTestExtensions.ShouldBe(holder.ImagePath, path);
        holder.SessionId.ShouldBe(processInfo.SessionId);
        holder.Restartable.ShouldBe(processInfo.IsRestartable);
    }
}
