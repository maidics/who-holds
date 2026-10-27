using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop.Services;

internal static class PortOwnerModuleQuery
{
    private static Result<NativeBuffer> QueryWithGrowingBuffer(
        QueryDelegate queryDelegate,
        string method
    )
    {
        SystemErrorCode? errorCode = null;
        uint size = 64 * 1024; // at least 64 KB

        for (int i = 0; i < 5; i++)
        {
            var buffer = new NativeBuffer((int)size);

            errorCode = queryDelegate.Invoke(buffer, ref size);

            if (errorCode is SystemErrorCode.ERROR_SUCCESS)
                return Result.Success(buffer);

            buffer.Dispose();

            if (errorCode is SystemErrorCode.ERROR_INSUFFICIENT_BUFFER)
            {
                size = (uint)Math.Ceiling(size * 1.25); // 25% headroom for the returned size
                continue;
            }

            return Result.Failure($"{method} query failed: {errorCode}.");
        }

        return Result.Failure(
            $"Buffer size never converged for {method} after 5 attempts (last buffer: {ByteFormat.Humanize(size)}, status: {errorCode})"
        );
    }

    private delegate SystemErrorCode QueryDelegate(NativeBuffer buffer, ref uint size);
}
