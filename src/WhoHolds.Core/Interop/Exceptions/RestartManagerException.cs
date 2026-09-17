using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop.Exceptions;

internal sealed class RestartManagerException : Exception
{
    private RestartManagerException(string message)
        : base(message) { }

    public static void ThrowIfOperationFailed(SystemErrorCode code, string method)
    {
        if (code is not SystemErrorCode.Success)
            throw new RestartManagerException($"'{method}' RM operation failed: {code}.");
    }
}
