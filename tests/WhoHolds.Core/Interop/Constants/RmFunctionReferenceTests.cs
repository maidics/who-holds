using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Constants;

internal sealed class RmFunctionReferenceTests
{
    private const string RmStartSession = nameof(Core.Interop.NativeMethods.RmStartSession);
    private const string RmRegisterResources = nameof(
        Core.Interop.NativeMethods.RmRegisterResources
    );
    private const string RmGetList = nameof(Core.Interop.NativeMethods.RmGetList);
    private const string RmEndSession = nameof(Core.Interop.NativeMethods.RmEndSession);

    [Test]
    public void ErrorCodeToResultShouldThrowIfFunctionIsUnknown()
    {
        Should.Throw<ArgumentException>(() =>
            RmFunctionReference.ErrorCodeToResult(SystemErrorCode.ERROR_SUCCESS, "test")
        );
    }

    [Test]
    [Arguments(RmStartSession)]
    [Arguments(RmRegisterResources)]
    [Arguments(RmGetList)]
    [Arguments(RmEndSession)]
    public void ErrorCodeToResultShouldReturnSucceededResultForSuccessCode(string function)
    {
        var result = RmFunctionReference.ErrorCodeToResult(
            SystemErrorCode.ERROR_SUCCESS,
            RmStartSession
        );
        result.ShouldBeResultedTo(ResultType.Success);
    }

    [Test]
    [Arguments(RmStartSession, SystemErrorCode.ERROR_MORE_DATA)]
    [Arguments(RmRegisterResources, SystemErrorCode.ERROR_MORE_DATA)]
    [Arguments(RmEndSession, SystemErrorCode.ERROR_MORE_DATA)]
    public void ErrorCodeToResultShouldThrowIfFunctionDoesNotReturnTheGivenErrorCode(
        string function,
        SystemErrorCode code
    )
    {
        Should.Throw<ArgumentException>(() =>
            RmFunctionReference.ErrorCodeToResult(code, function)
        );
    }

    [Test] // test supported codes here
    [MethodDataSource(nameof(ErrorCodeToResultShouldReturnResultForSupportedErrorCodesCases))]
    public void ErrorCodeToResultShouldReturnResultForSupportedErrorCodes(
        string function,
        HashSet<(SystemErrorCode code, ResultType resultType)> supported
    )
    {
        foreach (var tuple in supported)
        {
            var result = RmFunctionReference.ErrorCodeToResult(tuple.code, function);
            result.Type.ShouldBe(tuple.resultType);
        }
    }

    public static IEnumerable<(
        string,
        HashSet<(SystemErrorCode, ResultType)>
    )> ErrorCodeToResultShouldReturnResultForSupportedErrorCodesCases()
    {
        yield return (
            RmStartSession,
            [
                (SystemErrorCode.ERROR_SEM_TIMEOUT, ResultType.Timeout),
                (SystemErrorCode.ERROR_BAD_ARGUMENTS, ResultType.InternalError),
                (SystemErrorCode.ERROR_MAX_SESSIONS_REACHED, ResultType.Conflict),
                (SystemErrorCode.ERROR_WRITE_FAULT, ResultType.ExternalServiceError),
                (SystemErrorCode.ERROR_OUTOFMEMORY, ResultType.InternalError),
            ]
        );

        yield return (
            RmRegisterResources,
            [
                (SystemErrorCode.ERROR_SEM_TIMEOUT, ResultType.Timeout),
                (SystemErrorCode.ERROR_BAD_ARGUMENTS, ResultType.InternalError),
                (SystemErrorCode.ERROR_WRITE_FAULT, ResultType.ExternalServiceError),
                (SystemErrorCode.ERROR_OUTOFMEMORY, ResultType.InternalError),
                (SystemErrorCode.ERROR_INVALID_HANDLE, ResultType.InternalError),
            ]
        );

        yield return (
            RmGetList,
            [
                (SystemErrorCode.ERROR_MORE_DATA, ResultType.InternalError),
                (SystemErrorCode.ERROR_CANCELLED, ResultType.Canceled),
                (SystemErrorCode.ERROR_SEM_TIMEOUT, ResultType.Timeout),
                (SystemErrorCode.ERROR_BAD_ARGUMENTS, ResultType.InternalError),
                (SystemErrorCode.ERROR_WRITE_FAULT, ResultType.ExternalServiceError),
                (SystemErrorCode.ERROR_OUTOFMEMORY, ResultType.InternalError),
                (SystemErrorCode.ERROR_INVALID_HANDLE, ResultType.InternalError),
                (SystemErrorCode.ERROR_ACCESS_DENIED, ResultType.InternalError),
            ]
        );

        yield return (
            RmEndSession,
            [
                (SystemErrorCode.ERROR_SEM_TIMEOUT, ResultType.Timeout),
                (SystemErrorCode.ERROR_WRITE_FAULT, ResultType.ExternalServiceError),
                (SystemErrorCode.ERROR_OUTOFMEMORY, ResultType.InternalError),
                (SystemErrorCode.ERROR_INVALID_HANDLE, ResultType.InternalError),
            ]
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
        yield return (RmGetList, SystemErrorCode.ERROR_ACCESS_DENIED, "A path registered to the");
        yield return (
            RmRegisterResources,
            SystemErrorCode.ERROR_INVALID_HANDLE,
            "Invalid handle passed to "
        );
        yield return (
            RmStartSession,
            SystemErrorCode.ERROR_OUTOFMEMORY,
            "not enough memory was available."
        );
        yield return (
            RmStartSession,
            SystemErrorCode.ERROR_WRITE_FAULT,
            "failed to write to the Registry."
        );
        yield return (
            RmStartSession,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            "could not obtain Registry write mutex in time."
        );
        yield return (
            RmStartSession,
            SystemErrorCode.ERROR_BAD_ARGUMENTS,
            "One or more arguments passed to"
        );
        yield return (
            RmGetList,
            SystemErrorCode.ERROR_MORE_DATA,
            "Failed to allocate for data returned from"
        );
        yield return (
            RmStartSession,
            SystemErrorCode.ERROR_MAX_SESSIONS_REACHED,
            "sessions have been reached (64)."
        );
        yield return (RmGetList, SystemErrorCode.ERROR_CANCELLED, "The operation was canceled.");
    }
}
