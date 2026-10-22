namespace WhoHolds.Core.Interop.Kernel32Library.Enums;

[Flags]
internal enum ProcessAccessRights : uint
{
    DupHandle = 0x0040,
    QueryInformation = 0x0400,
}
