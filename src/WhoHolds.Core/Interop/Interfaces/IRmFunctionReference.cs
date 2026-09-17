using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop.Interfaces;

public interface IRmFunctionReference
{
    static abstract string FunctionName { get; }
    static abstract string DocumentationLink { get; }
    static abstract string GetErrorMessageByErrorCode(SystemErrorCode code);

    static abstract Result GetResultByErrorCode(SystemErrorCode code);

    static string[] GetResultErrorMessages(
        SystemErrorCode code,
        string documentationLink,
        params string[] errors
    ) =>
        [
            .. errors,
            GetSystemErrorCodeMessage(code),
            GetDocumentationErrorMessage(documentationLink),
        ];

    static string GetSystemErrorCodeMessage(SystemErrorCode code) =>
        $"{code.ToString()} ({(uint)code})";

    static string GetDocumentationErrorMessage(string documentationLink) =>
        $"More information about this function: {documentationLink}";
}
