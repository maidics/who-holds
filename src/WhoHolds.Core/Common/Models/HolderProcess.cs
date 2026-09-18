namespace WhoHolds.Core.Common.Models;

//TODO: add a command that describes the fields with sample values
public sealed record HolderProcess(
    int ProcessId, // .NET ProcessId is an int
    DateTime StartTime, // UTC time
    string? ApplicationName,
    string? ServiceShortName, // only set for services
    string ApplicationType, // interop enum
    List<string> ApplicationStatus, // interop flag enum => descibes history of actions taken by Restart Manager on the application
    int? SessionId, // session can be unknown (-1) => null
    bool Restartable
);
