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

    public override string ToString()
    {
        return $"{nameof(SystemHandleInformationEx)}\n\t- Object: 0x{Object:X}\n\t- UniqueProcessId: 0x{UniqueProcessId:X}\n\t- HandleValues: 0x{HandleValues:X}\n\t- GrantedAccess: {GrantedAccess}\n\t - CreatorBackTraceIndex: {CreatorBackTraceIndex}\n\t - ObjectTypeIndex: {ObjectTypeIndex}\n\t - Reserved: {Reserved}";
    }
}
