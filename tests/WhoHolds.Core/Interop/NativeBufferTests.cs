using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class NativeBufferTests
{
    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public void ShouldThrowIfSizeIsNotPositive(int size)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new NativeBuffer(size, SystemInformationClass.Basic)
        );
    }

    [Test]
    public void DisposeShouldBeIdempotent()
    {
        var buffer = new NativeBuffer(10, SystemInformationClass.Basic);

        buffer.Dispose();
        Should.NotThrow(buffer.Dispose);
        Should.NotThrow(buffer.Dispose);
    }

    [Test]
    public void ShouldProvideWritableMemoryForTheFullSize()
    {
        const int size = 4096;
        const SystemInformationClass systemInformationClass = SystemInformationClass.Basic;
        using var buffer = new NativeBuffer(size, systemInformationClass);

        for (int i = 0; i < size; i++)
            Marshal.WriteByte(buffer.Pointer, i, (byte)(i % 251));

        for (int i = 0; i < size; i++)
            Marshal.ReadByte(buffer.Pointer, i).ShouldBe((byte)(i % 251));

        buffer.Pointer.ShouldNotBe(IntPtr.Zero);
        buffer.Size.ShouldBe(size);
        buffer.SystemInformationClass.ShouldBe(systemInformationClass);
    }

    [Test]
    public void ShouldAllocateOnInstantiationAndReleaseOnDispose()
    {
        var buffer = new NativeBuffer(10, SystemInformationClass.Basic);

        buffer.Size.ShouldBe(10);
        buffer.Pointer.ShouldNotBe(IntPtr.Zero);

        buffer.Dispose();

        Should.Throw<ObjectDisposedException>(() => buffer.Pointer);
    }
}
