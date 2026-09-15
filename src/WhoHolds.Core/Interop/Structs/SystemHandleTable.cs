using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop.Structs;

internal readonly ref struct SystemHandleTable
{
    public readonly SystemHandleInformationEx Header;
    public readonly ReadOnlySpan<SystemHandleEntryEx> Entries;

    private static readonly int _headerSize = SystemHandleInformationEx.GetSize();
    private static readonly int _entrySize = SystemHandleEntryEx.GetSize();

    private SystemHandleTable(
        SystemHandleInformationEx header,
        ReadOnlySpan<SystemHandleEntryEx> entries
    )
    {
        Header = header;
        Entries = entries;
    }

    public static unsafe SystemHandleTable FromBuffer(NativeBuffer buffer, int returnLength)
    {
        if (buffer.SystemClass.Value is not SystemClass.SYSTEM_HANDLE_INFORMATION_EX)
            throw new InvalidOperationException(
                $"Buffer contains invalid class: {buffer.SystemClass}. Required: {SystemClass.SystemHandleInformationEx.ToString()}."
            );

        if (returnLength < _headerSize)
            throw new InvalidDataException(
                $"Buffer holds {returnLength} bytes, too small for a {nameof(SystemHandleInformationEx)} header ({ByteFormat.Humanize(_headerSize)})"
            );

        var header = Marshal.PtrToStructure<SystemHandleInformationEx>(buffer.Pointer);
        long count = header.NumberOfHandles.ToInt64();

        long needed = _headerSize + count * _entrySize;
        if (count < 0 || needed > returnLength)
            throw new InvalidDataException(
                $"Header claims {count:N0} handles, needing {ByteFormat.Humanize(needed)}, "
                    + $"but only {ByteFormat.Humanize(returnLength)} was returned. "
                    + "Layout mismatch — check the information class and the architecture."
            );

        return new SystemHandleTable(
            header,
            new ReadOnlySpan<SystemHandleEntryEx>((byte*)buffer.Pointer + _headerSize, (int)count)
        );
    }
}
