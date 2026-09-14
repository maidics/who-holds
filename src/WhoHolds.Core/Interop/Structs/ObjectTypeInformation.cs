using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
struct ObjectTypeInformation
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
}

[StructLayout(LayoutKind.Sequential)]
struct UnicodeString
{
    public ushort Length,
        MaximumLength;
    public IntPtr Buffer;
}

[StructLayout(LayoutKind.Sequential)]
struct GenericMapping
{
    public uint R,
        W,
        E,
        A;
}
