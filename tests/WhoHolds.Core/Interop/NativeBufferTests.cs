using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class NativeBufferTests
{
    [Test]
    [Arguments(-1)]
    [Arguments(0)]
    public void ShouldThrowIfSizeIsNotPositive(int size)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new NativeBuffer(size, SystemClass.SystemBasicInformation)
        );
    }

    [Test]
    public void DisposeShouldBeIdempotent()
    {
        var buffer = new NativeBuffer(10, SystemClass.SystemBasicInformation);

        buffer.Dispose();
        Should.NotThrow(buffer.Dispose);
        Should.NotThrow(buffer.Dispose);
    }

    [Test]
    public void ShouldProvideWritableMemoryForTheFullSize()
    {
        const int size = 4096;
        var systemClass = SystemClass.SystemBasicInformation;
        using var buffer = new NativeBuffer(size, systemClass);

        for (int i = 0; i < size; i++)
            Marshal.WriteByte(buffer.Pointer, i, (byte)(i % 251));

        for (int i = 0; i < size; i++)
            Marshal.ReadByte(buffer.Pointer, i).ShouldBe((byte)(i % 251));

        buffer.Pointer.ShouldNotBe(IntPtr.Zero);
        buffer.Size.ShouldBe(size);
        buffer.SystemClass.ShouldBe(systemClass);
    }

    [Test]
    public void ShouldAllocateOnInstantiationAndReleaseOnDispose()
    {
        var buffer = new NativeBuffer(10, SystemClass.SystemBasicInformation);

        buffer.Size.ShouldBe(10);
        buffer.Pointer.ShouldNotBe(IntPtr.Zero);

        buffer.Dispose();

        Should.Throw<ObjectDisposedException>(() => buffer.Pointer);
    }
}
