namespace WhoHolds.Core.Interop;

[Flags]
internal enum ProcessAccessRights : uint
{
    DupHandle = 0x0040,
    QueryInformation = 0x0400,
}
