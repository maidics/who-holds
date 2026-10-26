using System.Runtime.InteropServices;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop.Common;

// TODO: this can leak for caught exceptions
internal sealed class NativeBuffer : IDisposable
{
    private IntPtr _pointer;

    public IntPtr Pointer
    {
        get
        {
            ObjectDisposedException.ThrowIf(_pointer == IntPtr.Zero, this);
            return _pointer;
        }
    }

    public int Size { get; }
    public int Used { get; } // returnLength for reference

    public NativeBuffer(int size, int used)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(used, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(used, size);

        Size = size;
        Used = used;
        _pointer = Marshal.AllocHGlobal(size);
    }

    public void Dispose()
    {
        if (_pointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(Pointer);
            _pointer = IntPtr.Zero;
        }
    }

    public override string ToString()
    {
        if (_pointer == IntPtr.Zero)
            return $"{nameof(NativeBuffer)}(disposed, {Size:N0} bytes)";

        return $"{nameof(NativeBuffer)}(0x{_pointer:X}, {ByteFormat.Humanize(Size)})";
    }

    public static unsafe implicit operator ReadOnlySpan<byte>(NativeBuffer buffer) =>
        new((byte*)buffer.Pointer, buffer.Size);
}
