using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Constants;

internal sealed class RmFunctionReferenceTests
{
    private const string _rmStartSession = nameof(Core.Interop.NativeMethods.RmStartSession);
    private const string _rmRegisterResources = nameof(
        Core.Interop.NativeMethods.RmRegisterResources
    );
    private const string _rmGetList = nameof(Core.Interop.NativeMethods.RmGetList);
    private const string _rmEndSession = nameof(Core.Interop.NativeMethods.RmEndSession);

    [Test]
    public void ErrorCodeToResultShouldThrowIfFunctionIsUnknown()
    {
        Should.Throw<ArgumentException>(() =>
            RmFunctionReference.ErrorCodeToResult(SystemErrorCode.ERROR_SUCCESS, "test")
        );
    }

    [Test]
    [Arguments(_rmStartSession)]
    [Arguments(_rmRegisterResources)]
    [Arguments(_rmGetList)]
    [Arguments(_rmEndSession)]
    public void ErrorCodeToResultShouldReturnSucceededResultForSuccessCode(string function)
    {
        var result = RmFunctionReference.ErrorCodeToResult(
            SystemErrorCode.ERROR_SUCCESS,
            _rmStartSession
        );
        result.ShouldBeResultedTo(true);
    }

    [Test]
    [Arguments(_rmStartSession, SystemErrorCode.ERROR_MORE_DATA)]
    [Arguments(_rmRegisterResources, SystemErrorCode.ERROR_MORE_DATA)]
    [Arguments(_rmEndSession, SystemErrorCode.ERROR_MORE_DATA)]
    public void ErrorCodeToResultShouldThrowIfFunctionDoesNotReturnTheGivenErrorCode(
        string function,
        SystemErrorCode code
    )
    {
        Should.Throw<ArgumentException>(() =>
            RmFunctionReference.ErrorCodeToResult(code, function)
        );
    }

    [Test] // test error messages here
    [MethodDataSource(nameof(ErrorCodeToResultShouldReturnCorrectErrorMessagesForCodeCases))]
    public void ErrorCodeToResultShouldReturnCorrectErrorMessagesForCode(
        string function,
        SystemErrorCode code,
        string error
    )
    {
        var result = RmFunctionReference.ErrorCodeToResult(code, function);
        result.Errors[0].ShouldContain(error);
        result.Errors[1].ShouldContain($"{function} returned {code}: {(uint)code}.");
    }

    public static IEnumerable<(
        string,
        SystemErrorCode,
        string
    )> ErrorCodeToResultShouldReturnCorrectErrorMessagesForCodeCases()
    {
        yield return (_rmGetList, SystemErrorCode.ERROR_ACCESS_DENIED, "A path registered to the");
        yield return (
            _rmRegisterResources,
            SystemErrorCode.ERROR_INVALID_HANDLE,
            "Invalid handle passed to "
        );
        yield return (
            _rmStartSession,
            SystemErrorCode.ERROR_OUTOFMEMORY,
            "not enough memory was available."
        );
        yield return (
            _rmStartSession,
            SystemErrorCode.ERROR_WRITE_FAULT,
            "failed to write to the Registry."
        );
        yield return (
            _rmStartSession,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            "could not obtain Registry write mutex in time."
        );
        yield return (
            _rmStartSession,
            SystemErrorCode.ERROR_BAD_ARGUMENTS,
            "One or more arguments passed to"
        );
        yield return (
            _rmGetList,
            SystemErrorCode.ERROR_MORE_DATA,
            "Failed to allocate for data returned from"
        );
        yield return (
            _rmStartSession,
            SystemErrorCode.ERROR_MAX_SESSIONS_REACHED,
            "sessions have been reached (64)."
        );
        yield return (_rmGetList, SystemErrorCode.ERROR_CANCELLED, "The operation was canceled.");
    }
}
