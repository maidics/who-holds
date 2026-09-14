using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct ObjectTypeInformation : INtStruct
{
    public UnicodeString TypeName;
    public uint TotalNumberOfObjects,
        TotalNumberOfHandles;
    public uint TotalPagedPoolUsage,
        TotalNonPagedPoolUsage,
        TotalNamePoolUsage,
        TotalHandleTableUsage;
    public uint HighWaterNumberOfObjects,
        HighWaterNumberOfHandles;
    public uint HighWaterPagedPoolUsage,
        HighWaterNonPagedPoolUsage,
        HighWaterNamePoolUsage,
        HighWaterHandleTableUsage;
    public uint InvalidAttributes;
    public GenericMapping GenericMapping;
    public uint ValidAccessMask;
    public byte SecurityRequired,
        MaintainHandleCount;
    public byte TypeIndex; // valid on Windows 8.1+
    public byte ReservedByte;
    public uint PoolType,
        DefaultPagedPoolCharge,
        DefaultNonPagedPoolCharge;

    public static int GetSize() => Marshal.SizeOf<ObjectTypeInformation>();
}
