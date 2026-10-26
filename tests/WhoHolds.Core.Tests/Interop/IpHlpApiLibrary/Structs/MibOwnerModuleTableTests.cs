using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

public sealed class MibOwnerModuleTableTests
{
    private const int _entriesOffset = 8;

    private static readonly int _rowSize = MibTcp6RowOwnerModule.Size;

    private static int TableLength(int rows) => _entriesOffset + rows * _rowSize;

    private static byte[] CreateBuffer(uint count, int length)
    {
        var buffer = new byte[length];
        MemoryMarshal.Write(buffer, count);
        return buffer;
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(4)]
    [Arguments(7)]
    public void FromBufferShouldThrowIfBufferLengthIsLessThanEntriesOffset(int bufferLength)
    {
        var buffer = new byte[bufferLength];

        var ex = Should.Throw<InvalidDataException>(() =>
            MibOwnerModuleTable<MibTcp6RowOwnerModule>.FromBuffer(buffer)
        );
        ex.Message.ShouldStartWith("Buffer holds");
    }

    [Test]
    public void FromBufferShouldReturnNoRowsIfCountIsZero()
    {
        var buffer = CreateBuffer(0, TableLength(0));

        MibOwnerModuleTable<MibTcp6RowOwnerModule>.FromBuffer(buffer).Entries.Length.ShouldBe(0);
    }

    [Test]
    [Arguments(1u, 0)]
    [Arguments(3u, 0)]
    [Arguments(3u, 1)]
    [Arguments(3u, 4096)] // retry look headroom
    public void FromBufferShouldReturnCountRowsAndIgnoreTrailingBytes(uint count, int trailingBytes)
    {
        var buffer = CreateBuffer(count, TableLength((int)count) + trailingBytes);

        MibOwnerModuleTable<MibTcp6RowOwnerModule>
            .FromBuffer(buffer)
            .Entries.Length.ShouldBe((int)count);
    }

    [Test]
    [Arguments(1)]
    [Arguments(3)]
    public void FromBufferShouldThrowIfBufferIsShorterThanCountRows(int count)
    {
        var buffer = CreateBuffer((uint)count, TableLength(count) - 1);

        var ex = Should.Throw<InvalidDataException>(() =>
            MibOwnerModuleTable<MibTcp6RowOwnerModule>.FromBuffer(buffer)
        );
        ex.Message.ShouldStartWith("Header claims");
    }

    [Test]
    public void FromBufferShouldIgnoreHeaderPadding()
    {
        var buffer = CreateBuffer(1, TableLength(1));
        buffer.AsSpan(4, 4).Fill(0xFF); // garbage in padding must not leak into the count

        MibOwnerModuleTable<MibTcp6RowOwnerModule>.FromBuffer(buffer).Entries.Length.ShouldBe(1);
    }

    [Test]
    public void FromBufferShouldMapEachRowAtItsOffset()
    {
        var buffer = CreateBuffer(2, TableLength(2));
        var pidOffset = (int)
            Marshal.OffsetOf<MibTcp6RowOwnerModule>(nameof(MibTcp6RowOwnerModule.dwOwningPid));

        MemoryMarshal.Write(buffer.AsSpan(_entriesOffset + pidOffset), 100u);
        MemoryMarshal.Write(buffer.AsSpan(_entriesOffset + _rowSize + pidOffset), 200u);

        var rows = MibOwnerModuleTable<MibTcp6RowOwnerModule>.FromBuffer(buffer).Entries;

        rows[0].dwOwningPid.ShouldBe(100u);
        rows[1].dwOwningPid.ShouldBe(200u);
    }
}
