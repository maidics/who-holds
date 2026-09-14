using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop.Exceptions;

internal sealed class NtException : Exception
{
    private NtException(string message)
        : base(message) { }

    public static void ThrowIfUnsuccessful(NtStatus status, SystemInformationClass cls)
    {
        if (status is not NtStatus.Success)
            throw new NtException(
                $"Nt function call failed with status: {status}, SYSTEM_INFORMATION_CLASS: {cls}"
            );
    }
}
