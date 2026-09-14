using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core.Tests.TestInfrastructure;

internal static class TestHandleTableBuilder
{
    private static readonly int _headerSize = SystemHandleInformationEx.GetSize();
    private static readonly int _entrySize = SystemHandleEntryEx.GetSize();

    public static unsafe NativeBuffer BuildNativeBuffer(
        SystemHandleEntryEx[] entries,
        long? declaredCount = null, // pass invalid count
        int? bufferSize = null, // undersize allocation
        int? returnLength = null, // pass invalid bytes returned
        SystemInformationClass cls = SystemInformationClass.ExtendedHandle
    )
    {
        int size = bufferSize ?? _headerSize + entries.Length * _entrySize;
        var buffer = new NativeBuffer(size, cls);

        var p = (byte*)buffer.Pointer;
        new Span<byte>(p, size).Clear();

        var header = new SystemHandleInformationEx
        {
            NumberOfHandles = new IntPtr(declaredCount ?? entries.Length),
            Reserved = IntPtr.Zero,
        };

        Unsafe.Write(p, header);

        var dest = new Span<SystemHandleEntryEx>(p + _headerSize, entries.Length);
        entries.CopyTo(dest);

        return buffer;
    }

    public static SystemHandleEntryEx BuildEntry(
        int pid = 1234,
        long handle = 0x1F4,
        ushort typeIndex = 37,
        uint access = 0x120089,
        long obj = 0x7FFF_0000_1000
    ) =>
        new()
        {
            Object = new IntPtr(obj),
            UniqueProcessId = new IntPtr(pid),
            HandleValue = new IntPtr(handle),
            GrantedAccess = access,
            ObjectTypeIndex = typeIndex,
            CreatorBackTraceIndex = 0,
            HandleAttributes = 0,
            Reserved = 0,
        };
}
