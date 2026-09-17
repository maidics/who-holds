using System.Collections.Frozen;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop.Constants;

internal static class RmFunctionReference
{
    private const string ApiName = "Restart Manager";

    public static Result ErrorCodeToResult(SystemErrorCode code, string function)
    {
        if (!_functions.Contains(function))
            throw new ArgumentException($"Unknown {ApiName} function: " + function);

        if (code is SystemErrorCode.ERROR_SUCCESS)
            return Result.Success();

        var errors = GetErrors(code, function);

        if (!_resultFailureFactoryMethods.TryGetValue(code, out var resultFactory))
            throw new ArgumentException($"No {nameof(Result)} factory method found for {code}.");

        return resultFactory(errors);
    }

    private static string[] GetErrors(SystemErrorCode code, string function)
    {
        if (!_returnedErrorCodes[function].Contains(code))
            throw new ArgumentException(
                $"Unknown {nameof(SystemErrorCode)} for function: {function}."
            );

        if (!_errorMessages.TryGetValue(code, out var error))
            throw new ArgumentException($"Unknown error: {code}.");

        return
        [
            error,
            $"{ApiName} {function} returned {code}: {(uint)code}.",
            .. _documentations.TryGetValue(function, out var doc)
                ? new[] { $"More information about this function: {doc}" }
                : [],
        ];
    }

    private static readonly Dictionary<string, HashSet<SystemErrorCode>> _returnedErrorCodes = new()
    {
        [nameof(NativeMethods.RmStartSession)] =
        [
            SystemErrorCode.ERROR_SUCCESS,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            SystemErrorCode.ERROR_BAD_ARGUMENTS,
            SystemErrorCode.ERROR_MAX_SESSIONS_REACHED,
            SystemErrorCode.ERROR_WRITE_FAULT,
            SystemErrorCode.ERROR_OUTOFMEMORY,
        ],
        [nameof(NativeMethods.RmRegisterResources)] =
        [
            SystemErrorCode.ERROR_SUCCESS,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            SystemErrorCode.ERROR_BAD_ARGUMENTS,
            SystemErrorCode.ERROR_WRITE_FAULT,
            SystemErrorCode.ERROR_OUTOFMEMORY,
            SystemErrorCode.ERROR_INVALID_HANDLE,
        ],
        [nameof(NativeMethods.RmGetList)] =
        [
            SystemErrorCode.ERROR_SUCCESS,
            SystemErrorCode.ERROR_MORE_DATA,
            SystemErrorCode.ERROR_CANCELLED,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            SystemErrorCode.ERROR_BAD_ARGUMENTS,
            SystemErrorCode.ERROR_WRITE_FAULT,
            SystemErrorCode.ERROR_OUTOFMEMORY,
            SystemErrorCode.ERROR_INVALID_HANDLE,
            SystemErrorCode.ERROR_ACCESS_DENIED,
        ],
        [nameof(NativeMethods.RmEndSession)] =
        [
            SystemErrorCode.ERROR_SUCCESS,
            SystemErrorCode.ERROR_SEM_TIMEOUT,
            SystemErrorCode.ERROR_WRITE_FAULT,
            SystemErrorCode.ERROR_OUTOFMEMORY,
            SystemErrorCode.ERROR_INVALID_HANDLE,
        ],
    };

    private static readonly Dictionary<SystemErrorCode, string> _errorMessages = new()
    {
        [SystemErrorCode.ERROR_SEM_TIMEOUT] =
            $"{ApiName} function could not obtain Registry write mutex in time. A system restart is recommended because further use of {ApiName} is likely to fail.",
        [SystemErrorCode.ERROR_WRITE_FAULT] = $"{ApiName} failed to write to the Registry.",
        [SystemErrorCode.ERROR_OUTOFMEMORY] =
            $"{ApiName} operation could not complete because not enough memory was available.",
        [SystemErrorCode.ERROR_BAD_ARGUMENTS] =
            $"One or more arguments passed to {ApiName} is not correct. Please contact the maintainer of this application.",
        [SystemErrorCode.ERROR_INVALID_HANDLE] = $"Invalid handle passed to {ApiName}.",
        [SystemErrorCode.ERROR_ACCESS_DENIED] =
            $"A path registered to the {ApiName} session is a directory.",
        [SystemErrorCode.ERROR_MORE_DATA] =
            $"Failed to allocate for data returned from {nameof(NativeMethods.RmGetList)}.",
        [SystemErrorCode.ERROR_MAX_SESSIONS_REACHED] =
            $"Maximum number of {ApiName} sessions have been reached (64). End your sessions or restart your PC.",
        [SystemErrorCode.ERROR_CANCELLED] = "The operation was canceled.",
    };

    private static readonly Dictionary<string, string> _documentations = new()
    {
        [nameof(NativeMethods.RmStartSession)] =
            "https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmstartsession",
        [nameof(NativeMethods.RmRegisterResources)] =
            "https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmregisterresources",
        [nameof(NativeMethods.RmGetList)] =
            "https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmgetlist",
        [nameof(NativeMethods.RmEndSession)] =
            "https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmendsession",
    };

    private static readonly Dictionary<
        SystemErrorCode,
        Func<string[], ResultFailure>
    > _resultFailureFactoryMethods = new()
    {
        [SystemErrorCode.ERROR_ACCESS_DENIED] = Result.InternalError, // should be against - registering a directory as resource
        [SystemErrorCode.ERROR_INVALID_HANDLE] = Result.InternalError,
        [SystemErrorCode.ERROR_OUTOFMEMORY] = Result.InternalError,
        [SystemErrorCode.ERROR_WRITE_FAULT] = Result.ExternalServiceError,
        [SystemErrorCode.ERROR_SEM_TIMEOUT] = Result.Timeout,
        [SystemErrorCode.ERROR_BAD_ARGUMENTS] = Result.InternalError,
        [SystemErrorCode.ERROR_MAX_SESSIONS_REACHED] = Result.Conflict,
        [SystemErrorCode.ERROR_CANCELLED] = Result.Canceled,
        [SystemErrorCode.ERROR_MORE_DATA] = Result.InternalError,
    };

    private static readonly FrozenSet<string> _functions = _documentations.Keys.ToFrozenSet();
}
