using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

// extended system handle entry
[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleEntryEx
{
    public IntPtr Object;
    public IntPtr UniqueProcessId;
    public IntPtr HandleValues;
    public uint GrantedAccess;
    public ushort CreatorBackTraceIndex;
    public ushort ObjectTypeIndex;
    public uint HandleAttributes;
    public uint Reserved;
}
