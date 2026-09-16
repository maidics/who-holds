namespace WhoHolds.Core.Interop.Constants;

internal static class RestartManagerLimits
{
    /// <summary>
    /// Character count of the text-encoded Restart Manager session key, excluding the
    /// terminating null. Defined in <c>RestartManager.h</c> as
    /// <c>RM_SESSION_KEY_LEN * 2</c>, where <c>RM_SESSION_KEY_LEN</c> is
    /// <c>sizeof(GUID)</c>: a 16-byte GUID hex-encodes to 32 characters.
    /// </summary>
    /// <seealso href="https://github.com/tpn/winsdk-10/blob/master/Include/10.0.10240.0/um/RestartManager.h">
    /// RestartManager.h (Windows SDK 10.0.10240.0)
    /// </seealso>
    public const int CCH_RM_SESSION_KEY = 32;

    public const int SessionKeyBufferLength = CCH_RM_SESSION_KEY + 1;
}
