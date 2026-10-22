using WhoHolds.Core.Interop.NtDllLibrary.Enums;
using WhoHolds.Core.Interop.NtDllLibrary.Exceptions;
using WhoHolds.Core.Interop.NtDllLibrary.Structs;

namespace WhoHolds.Core.Tests.Interop.NtDllLibrary.Exceptions;

internal sealed class NtExceptionTests
{
    [Test]
    public void ShouldThrowIfUnsuccessful()
    {
        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(
                NtStatus.InvalidInfoClass,
                SystemClass.SystemBasicInformation
            )
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(
                NtStatus.InfoLengthMismatch,
                SystemClass.SystemBasicInformation
            )
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(
                NtStatus.AccessDenied,
                SystemClass.SystemBasicInformation
            )
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(
                NtStatus.InvalidHandle,
                SystemClass.SystemBasicInformation
            )
        );
    }

    [Test]
    public void ShouldNotThrowIfSuccessful()
    {
        NtException.ThrowIfUnsuccessful(NtStatus.Success, SystemClass.SystemBasicInformation);
    }
}
