using System.Runtime.InteropServices;
using Shouldly;
using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.Interop;

public sealed class NativeBufferTests
{
    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public void ShouldThrowIfSizeIsNotPositive(int size)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new NativeBuffer(size));
    }

    [Test]
    public void DisposeShouldBeIdempotent()
    {
        var buffer = new NativeBuffer(10);

        buffer.Dispose();
        Should.NotThrow(buffer.Dispose);
        Should.NotThrow(buffer.Dispose);
    }

    [Test]
    public void ShouldProvideWritableMemoryForTheFullSize()
    {
        const int size = 4096;
        using var buffer = new NativeBuffer(size);

        for (int i = 0; i < size; i++)
            Marshal.WriteByte(buffer.Pointer, i, (byte)(i % 251));

        for (int i = 0; i < size; i++)
            Marshal.ReadByte(buffer.Pointer, i).ShouldBe((byte)(i % 251));
    }

    [Test]
    public void ShouldAllocateOnInstantiationAndReleaseOnDispose()
    {
        var buffer = new NativeBuffer(10);

        buffer.Size.ShouldBe(10);
        buffer.Pointer.ShouldNotBe(IntPtr.Zero);

        buffer.Dispose();

        Should.Throw<ObjectDisposedException>(() => buffer.Pointer);
    }
}
