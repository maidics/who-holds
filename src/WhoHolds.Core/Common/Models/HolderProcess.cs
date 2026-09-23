namespace WhoHolds.Core.Common.Models;

public sealed record HolderProcess(
    int ProcessId, // .NET ProcessId is an int
    DateTime StartTime, // UTC time
    string? ApplicationName,
    string? ServiceShortName, // only set for services
    string ApplicationType, // interop enum
    List<string> ApplicationStatus, // interop flag enum => descibes history of actions taken by Restart Manager on the application
    int? SessionId, // session can be unknown (-1) => null
    bool Restartable
)
{
    public override string ToString()
    {
        return $"Holder:"
            + $"\n\t- Process: {ProcessId}"
            + $"\n\t- Start time: {StartTime}"
            + $"\n\t- Application: {ApplicationName}"
            + $"\n\t- Application type: {ApplicationType}"
            + $"\n\t- Application statuses: {string.Join(", ", ApplicationStatus)}"
            + $"\n\t- Service: {ServiceShortName}"
            + $"\n\t- Session id: {SessionId}"
            + $"\n\t- Restartable: {Restartable}";
    }

    public static string DescribeFields()
    {
        return $"Holder:"
            + $"\n\t- Process: holder process id"
            + $"\n\t- Start time: process start time"
            + $"\n\t- Application: holder application name (null if holder is not an application)"
            + $"\n\t- Application type: holder application type (UnknownApp, MainWindow, OtherWindow, Service, Explorer, Console, Critical)"
            + $"\n\t- Application statuses: history of actions taken by the Restart Manager"
            + $"\n\t- Service: service short name (null if holder is not a service)"
            + $"\n\t- Session id: terminal session id of the process (null if Restart Manager cannot determine it)"
            + $"\n\t- Restartable: true if the process can be restarted by the Restart Manager, otherwise false";
    }
}
