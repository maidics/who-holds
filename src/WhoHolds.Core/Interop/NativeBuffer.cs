using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop;

internal sealed class NativeBuffer : IDisposable
{
    private IntPtr _pointer;

    public NativeBuffer(int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);

        Size = size;
        _pointer = Marshal.AllocHGlobal(size);
    }

    public IntPtr Pointer
    {
        get
        {
            ObjectDisposedException.ThrowIf(_pointer == IntPtr.Zero, this);
            return _pointer;
        }
    }

    public int Size { get; }

    public void Dispose()
    {
        if (_pointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(Pointer);
            _pointer = IntPtr.Zero;
        }
    }
}
