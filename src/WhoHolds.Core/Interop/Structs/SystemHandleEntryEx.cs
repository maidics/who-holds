using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

// extended system handle entry
[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleEntryEx : INativeSized<SystemHandleEntryEx>
{
    public IntPtr Object;
    public IntPtr UniqueProcessId;
    public IntPtr HandleValue;
    public uint GrantedAccess;
    public ushort CreatorBackTraceIndex;
    public ushort ObjectTypeIndex;
    public uint HandleAttributes;
    public uint Reserved;

    public override string ToString()
    {
        return $"{nameof(SystemHandleEntryEx)}:\n\t- Object: 0x{Object:X}\n\t- UniqueProcessId: 0x{UniqueProcessId:X}\n\t- HandleValue: 0x{HandleValue:X}\n\t- GrantedAccess: {GrantedAccess}\n\t- CreatorBackTraceIndex: {CreatorBackTraceIndex}\n\t- ObjectTypeIndex: {ObjectTypeIndex}\n\t- Reserved: {Reserved}";
    }

    public static int Size => Marshal.SizeOf<SystemHandleEntryEx>();
}
