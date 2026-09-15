using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop;

internal static class SystemQuery
{
    private const int MaxBufferSize = int.MaxValue;

    public static NativeBuffer QueryWithGrowingBuffer(
        SystemQueryDelegate query,
        SystemClass systemClass,
        out uint returnLength,
        int initialSize = 1 << 20
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(initialSize, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(initialSize, MaxBufferSize);

        int size = initialSize;
        returnLength = 0;
        NtStatus? status = null;

        for (int attempt = 0; attempt < 8; attempt++)
        {
            var buffer = new NativeBuffer(size, systemClass);
            status = query(systemClass, buffer.Pointer, size, out returnLength);

            if (status is NtStatus.Success) //TODO: handle more cases if required: STATUS_PENDING, STATUS_MORE_ENTRIES, STATUS_SOME_NOT_MAPPED
                return buffer;

            buffer.Dispose();

            if (status is not (NtStatus.InfoLengthMismatch or NtStatus.BufferTooSmall))
                NtException.ThrowIfUnsuccessful(status.Value, systemClass);

            if (!TryGrowBuffer(size, returnLength, out int next))
                throw new InvalidDataException(
                    $"Data requires more than {MaxBufferSize} bytes ({ByteFormat.Humanize(MaxBufferSize)})."
                );

            size = next;
        }

        throw new InvalidOperationException(
            $"Buffer size never converged for {systemClass} after 8 attempts"
                + $" (last {ByteFormat.Humanize(size)}), status: {status}."
        );
    }

    private static bool TryGrowBuffer(int current, uint returnLength, out int result)
    {
        long r = returnLength > current ? returnLength + (long)returnLength / 4 : (long)current * 2;

        if (r > MaxBufferSize)
            r = MaxBufferSize;

        if (r <= current)
        {
            result = 0;
            return false;
        }

        result = (int)r;
        return true;
    }
}
