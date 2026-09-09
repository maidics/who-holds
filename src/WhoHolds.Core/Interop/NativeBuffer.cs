using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop;

internal sealed class NativeBuffer : IDisposable
{
    private IntPtr _pointer;

    public NativeBuffer(int size, SystemInformationClass cls)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);

        Size = size;
        _pointer = Marshal.AllocHGlobal(size);
        SystemInformationClass = cls;
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
    public SystemInformationClass SystemInformationClass { get; }

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
}
