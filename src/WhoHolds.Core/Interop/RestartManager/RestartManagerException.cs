using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

internal sealed class RestartManagerException : Exception
{
    private RestartManagerException(string message)
        : base(message) { }

    public static void ThrowIfOperationFailed(SystemErrorCode code, string method)
    {
        if (code is not SystemErrorCode.ERROR_SUCCESS)
            throw new RestartManagerException($"'{method}' RM operation failed: {code}.");
    }
}
