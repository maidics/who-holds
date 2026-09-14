using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;

namespace WhoHolds.Core.Interop;

internal static class SystemQuery
{
    public static NativeBuffer QueryWithGrowingBuffer(
        SystemQueryDelegate query,
        SystemInformationClass cls,
        out int returnLength,
        int initialSize = 1 << 20
    )
    {
        int size = initialSize;

        for (int attempt = 0; attempt < 8; attempt++)
        {
            var buffer = new NativeBuffer(size, cls);
            var status = query(cls, buffer.Pointer, size, out returnLength);

            if (status is NtStatus.Success)
                return buffer;

            buffer.Dispose();

            if (status is NtStatus.InfoLengthMismatch)
            {
                size = returnLength > size ? returnLength + (returnLength / 4) : size * 2;
                continue;
            }

            NtException.ThrowIfUnsuccessful(status, cls);
        }

        throw new InvalidOperationException($"Buffer size never converged: {cls}.");
    }
}
