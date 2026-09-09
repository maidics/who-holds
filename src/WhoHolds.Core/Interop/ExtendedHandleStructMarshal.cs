using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop;

internal static class ExtendedHandleStructMarshal
{
    private static readonly int _headerSize = Marshal.SizeOf<SystemHandleInformationEx>();
    private static readonly int _entrySize = Marshal.SizeOf<SystemHandleEntryEx>();

    public static unsafe ReadOnlySpan<SystemHandleEntryEx> EntriesToStruct(
        NativeBuffer buffer,
        int returnLength
    )
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.SystemInformationClass is not SystemInformationClass.ExtendedHandle)
            throw new InvalidOperationException(
                $"Buffer contains invalid class: {buffer.SystemInformationClass}. Required: {SystemInformationClass.ExtendedHandle}."
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

        return new ReadOnlySpan<SystemHandleEntryEx>(
            (byte*)buffer.Pointer + _headerSize,
            (int)count
        );
    }
}
