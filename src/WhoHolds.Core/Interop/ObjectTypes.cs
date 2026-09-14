using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core.Interop;

public static class ObjectTypes
{
    private static bool _initialized;
    private static readonly Dictionary<ushort, string> _map = new();

    public static string GetName(ushort objectType)
    {
        if (!_initialized)
            BuildMap();

        if (_map.TryGetValue(objectType, out var typeName))
            return typeName;

        BuildMap();

        return _map.TryGetValue(objectType, out typeName)
            ? typeName
            : throw new ArgumentException(
                $"Failed to find object name for object type: {objectType}."
            );
    }

    private static void BuildMap()
    {
        var buffer = SystemQuery.QueryWithGrowingBuffer(
            (cls, buffer, bufferLength, out ret) =>
                NativeMethods.NtQueryObject(IntPtr.Zero, cls, buffer, bufferLength, out ret),
            SystemInformationClass.ObjectTypesInformation,
            out int _
        );

        uint count = (uint)Marshal.ReadInt32(buffer.Pointer);
        IntPtr p = IntPtr.Add(buffer.Pointer, IntPtr.Size);

        for (uint i = 0; i < count; i++)
        {
            var info = Marshal.PtrToStructure<ObjectTypeInformation>(p);
            string name = Marshal.PtrToStringUni(info.TypeName.Buffer, info.TypeName.Length / 2);

            _map[info.TypeIndex] = name;

            long next =
                p.ToInt64() + Marshal.SizeOf<ObjectTypeInformation>() + info.TypeName.MaximumLength;
            next = (next + IntPtr.Size - 1) & ~((long)IntPtr.Size - 1);
            p = new IntPtr(next);
        }

        buffer.Dispose();

        _initialized = true;
    }
}
