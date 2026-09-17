using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Constants;

internal abstract class RmStartSessionReference : IRmFunctionReference
{
    public static string FunctionName => nameof(NativeMethods.RmStartSession);

    public static string DocumentationLink =>
        "https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmstartsession";

    public static string GetErrorMessageByErrorCode(SystemErrorCode code)
    {
        return code switch
        {
            SystemErrorCode.ERROR_SEM_TIMEOUT =>
                $"Restart Manager {nameof(NativeMethods.RmStartSession)} function could not obtain Registry write mutex in time. A system restart is recommended because further use of Restart Manager is likely to fail.",
            SystemErrorCode.ERROR_BAD_ARGUMENTS =>
                $"One or more arguments passed to {nameof(NativeMethods.RmStartSession)} is not correct. Please contact the maintainer of this application.",
            SystemErrorCode.ERROR_MAX_SESSIONS_REACHED =>
                "Maximum number of Restart Manager sessions have been reached (64). End your sessions or restart your PC.",
            SystemErrorCode.ERROR_WRITE_FAULT => "Restart Manager failed to write to the Registry.",
            SystemErrorCode.ERROR_OUTOFMEMORY =>
                "Restart Manager operation could not complete because not enough memory was available.",
            _ => throw new ArgumentException($"Unknown {FunctionName} error code: {code}"),
        };
    }

    public static Result GetResultByErrorCode(SystemErrorCode code)
    {
        if (code is SystemErrorCode.ERROR_SUCCESS)
            return Result.Success();

        var errors = IRmFunctionReference.GetResultErrorMessages(
            code,
            DocumentationLink,
            GetErrorMessageByErrorCode(code)
        );

        Func<string[], ResultFailure> resultFactory = code switch
        {
            SystemErrorCode.ERROR_SEM_TIMEOUT => Result.Timeout,
            SystemErrorCode.ERROR_BAD_ARGUMENTS => Result.InternalError,
            SystemErrorCode.ERROR_MAX_SESSIONS_REACHED => Result.RuleViolation,
            SystemErrorCode.ERROR_WRITE_FAULT => Result.ExternalServiceError,
            SystemErrorCode.ERROR_OUTOFMEMORY => Result.ExternalServiceError,
            _ => throw new ArgumentException($"Unknown error code for {FunctionName}."),
        };

        return resultFactory(errors);
    }
}
