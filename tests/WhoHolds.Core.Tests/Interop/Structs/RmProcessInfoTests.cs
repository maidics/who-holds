using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

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

        processInfo.AppName.ShouldBe(expectedAppName);
        processInfo.ServiceShortName.ShouldBe(expectedServiceShortName);
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

        processInfo.IsRestartable.ShouldBe(expected);
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

        processInfo.SessionId.ShouldBe(expected);
    }

    [Test]
    public void ToHolderProcessShouldMapCorrectly()
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

        var holder = processInfo.ToHolderProcess();
        holder.ProcessId.ShouldBe((int)processInfo.Process.dwProcessId);
        holder.StartTime.ShouldBe(processInfo.Process.ProcessStartTime.ToDateTimeUtc());
        holder.ApplicationName.ShouldBe(processInfo.AppName);
        holder.ServiceShortName.ShouldBe(processInfo.ServiceShortName);
        holder.ApplicationType.ShouldBe(processInfo.ApplicationType.ToString());
        holder.ApplicationStatus.ShouldBe(["Running"]);
        holder.SessionId.ShouldBe(processInfo.SessionId);
        holder.Restartable.ShouldBe(processInfo.IsRestartable);
    }
}
