using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;

namespace WhoHolds.Core.Interop;

internal static class SystemQuery
{
    // 1 << 20 shifts the bit position of 1 twenty positions to the left: 2 to the power of 20
    public static NativeBuffer Query(SystemInformationClass cls, int initialSize = 1 << 20)
    {
        int size = initialSize;

        for (int attempt = 0; attempt < 8; attempt++) //TODO: add
        {
            var buffer = new NativeBuffer(size);
            var status = NativeMethods.NtQuerySystemInfo(cls, buffer.Pointer, size, out int needed);

            if (status is NtStatus.Success)
                return buffer;

            buffer.Dispose();

            NtException.ThrowIfInfoLengthMismatch(status, cls);

            size = Math.Max(needed, size * 2);
        }

        throw new InvalidOperationException($"Buffer size never converged: {cls}.");
    }
}
