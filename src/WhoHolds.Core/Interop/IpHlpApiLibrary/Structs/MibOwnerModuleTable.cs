using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

internal readonly struct MibOwnerModuleTable<TRow>
    where TRow : unmanaged
{
    // header is 4 bytes with 4 bytes of padding
    private const int _entriesOffset = sizeof(ulong); // header contains entry count only - this applies to all ...RowOwnerModule structs

    public readonly IReadOnlyList<TRow> Entries;

    private MibOwnerModuleTable(IReadOnlyList<TRow> entries) => Entries = entries;

    public static MibOwnerModuleTable<TRow> FromBuffer(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < _entriesOffset)
            throw new InvalidDataException(
                $"Buffer holds {buffer.Length} bytes, too small for a table header ({_entriesOffset} bytes)."
            );

        uint count = MemoryMarshal.Read<uint>(buffer);
        long needed = _entriesOffset + count * Unsafe.SizeOf<TRow>();

        if (needed > buffer.Length)
            throw new InvalidDataException(
                $"Header claims {count:N0} rows, needing {ByteFormat.Humanize(needed)}, "
                    + $"but the buffer holds {ByteFormat.Humanize(buffer.Length)}. "
            );

        return new MibOwnerModuleTable<TRow>(
            MemoryMarshal.Cast<byte, TRow>(buffer[_entriesOffset..(int)needed]).ToArray()
        );
    }
}
