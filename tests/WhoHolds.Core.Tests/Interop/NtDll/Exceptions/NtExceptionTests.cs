using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.Interop;

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
