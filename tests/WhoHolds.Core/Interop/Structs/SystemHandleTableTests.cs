using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

internal sealed class SystemHandleTableTests
{
    [Test]
    public void FromBufferShouldThrowIfBufferContainsInvalidSystemInformationClass()
    {
        using var buffer = new NativeBuffer(10, SystemClass.SystemBasicInformation);

        var ex = Should.Throw<InvalidOperationException>(() =>
            SystemHandleTable.FromBuffer(buffer, 10)
        );
        ex.Message.ShouldStartWith("Buffer contains invalid class:");
    }

    [Test]
    public void FromBufferShouldThrowIfReturnLengthIsLessThanHeaderSize()
    {
        using var buffer = new NativeBuffer(10, SystemClass.SystemHandleInformationEx);

        var headerSize = Marshal.SizeOf<SystemHandleInformationEx>();

        var ex = Should.Throw<InvalidDataException>(() =>
            SystemHandleTable.FromBuffer(buffer, (uint)headerSize - 1)
        );
        ex.Message.ShouldStartWith("Buffer holds");
    }

    [Test]
    public void FromBufferShouldThrowIfCountIsLessThan0()
    {
        using var buffer = TestHandleTableBuilder.BuildNativeBuffer([], -1);

        var ex = Should.Throw<InvalidDataException>(() =>
            SystemHandleTable.FromBuffer(buffer, (uint)SystemHandleInformationEx.GetSize())
        );
        ex.Message.ShouldStartWith("Header claims");
    }

    [Test]
    public void ShouldReadEntriesInOrder()
    {
        var entries = new[]
        {
            TestHandleTableBuilder.BuildEntry(pid: 4, handle: 0x04, typeIndex: 37),
            TestHandleTableBuilder.BuildEntry(pid: 1234, handle: 0x1F4, typeIndex: 7),
            TestHandleTableBuilder.BuildEntry(pid: 9999, handle: 0xABC, typeIndex: 3),
        };

        using var buffer = TestHandleTableBuilder.BuildNativeBuffer(entries);
        var table = SystemHandleTable.FromBuffer(buffer, (uint)buffer.Size);

        table.Entries.Length.ShouldBe(3);
        table.Entries[0].UniqueProcessId.ShouldBe(4);
        table.Entries[0].HandleValue.ShouldBe(0x04);
        table.Entries[0].ObjectTypeIndex.ShouldBe((ushort)37);

        table.Header.NumberOfHandles.ShouldBe(table.Entries.Length);
    }
}
