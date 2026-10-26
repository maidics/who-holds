using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Tests.Interop.Common;

internal sealed class NativeBufferTests
{
    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public void ShouldThrowIfSizeIsNotPositive(int size)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new NativeBuffer(size, 0));
    }

    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public void ShouldThrowIfUsedIsNotPositive(int used)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new NativeBuffer(int.MaxValue, used));
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(10, 20)]
    public void ShouldThrowIfUsedIsGreaterThanSize(int size, int used)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new NativeBuffer(size, used));
    }

    [Test]
    public void DisposeShouldBeIdempotent()
    {
        var buffer = new NativeBuffer(10, 1);

        buffer.Dispose();
        Should.NotThrow(buffer.Dispose);
        Should.NotThrow(buffer.Dispose);
    }

    [Test]
    public void ShouldProvideWritableMemoryForTheFullSize()
    {
        const int size = 4096;
        using var buffer = new NativeBuffer(size, 1);

        for (int i = 0; i < size; i++)
            Marshal.WriteByte(buffer.Pointer, i, (byte)(i % 251));

        for (int i = 0; i < size; i++)
            Marshal.ReadByte(buffer.Pointer, i).ShouldBe((byte)(i % 251));

        buffer.Pointer.ShouldNotBe(IntPtr.Zero);
        buffer.Size.ShouldBe(size);
    }

    [Test]
    public void ShouldAllocateOnInstantiationAndReleaseOnDispose()
    {
        var buffer = new NativeBuffer(10, 1);

        buffer.Size.ShouldBe(10);
        buffer.Pointer.ShouldNotBe(IntPtr.Zero);

        buffer.Dispose();

        Should.Throw<ObjectDisposedException>(() => buffer.Pointer);
    }
}
