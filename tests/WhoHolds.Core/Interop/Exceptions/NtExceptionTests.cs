using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;

namespace WhoHolds.Core.Tests.Interop.Exceptions;

internal sealed class NtExceptionTests
{
    [Test]
    public void ShouldThrowIfUnsuccessful()
    {
        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(NtStatus.InvalidInfoClass, SystemInformationClass.Basic)
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(
                NtStatus.InfoLengthMismatch,
                SystemInformationClass.Basic
            )
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(NtStatus.AccessDenied, SystemInformationClass.Basic)
        );

        Should.Throw<NtException>(() =>
            NtException.ThrowIfUnsuccessful(NtStatus.InvalidHandle, SystemInformationClass.Basic)
        );
    }

    [Test]
    public void ShouldNotThrowIfSuccessful()
    {
        NtException.ThrowIfUnsuccessful(NtStatus.Success, SystemInformationClass.Basic);
    }
}
