using WhoHolds.Core.Interop.NtDllLibrary.Enums;
using WhoHolds.Core.Interop.NtDllLibrary.Structs;

namespace WhoHolds.Core.Interop.NtDllLibrary.Exceptions;

internal sealed class NtException : Exception
{
    private NtException(string message)
        : base(message) { }

    public static void ThrowIfUnsuccessful(NtStatus status, SystemClass cls)
    {
        if (status is not NtStatus.Success)
            throw new NtException(
                $"Nt function call failed with status: {status}, class: {cls.ToString()}"
            );
    }
}
