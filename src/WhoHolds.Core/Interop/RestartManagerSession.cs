using System.Diagnostics;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core.Interop;

internal sealed class RestartManagerSession : IDisposable // file paths have to normalized before or _filePaths have to passed in Start()
{
    private bool _disposed;
    private bool _started; // default for uint 0 which is a valid handle so this is required to know whether the session has been started
    private uint _sessionHandle;
    private readonly string[] _filePaths;

    public RestartManagerSession(string[] filePaths)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(filePaths.Length, 0, nameof(filePaths));
        // limit should be uint.MaxValue but this will run out of memory before that so the cap will be the user's memory instead of a constant

        var invalid = filePaths.Where(p => !Path.Exists(p)).ToList();

        if (invalid.Count != 0) // this does not have to be more robust because the orchestrator of this class should do checks for the file paths
            throw new ArgumentException(
                $"Passed file paths are invalid: {string.Join(", ", invalid)}.",
                nameof(filePaths)
            );

        _filePaths = filePaths;
    }

    public Result Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_started)
            throw new InvalidOperationException(
                $"{nameof(RestartManagerSession)} has already been started."
            );

        Span<char> sessionKeyBuffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = NativeMethods.RmStartSession(out _sessionHandle, 0, sessionKeyBuffer);

        if (startCode is not SystemErrorCode.ERROR_SUCCESS)
            return RmFunctionReference.ErrorCodeToResult(
                startCode,
                nameof(NativeMethods.RmStartSession)
            );

        var registerCode = RegisterResourcesSafe(_sessionHandle);

        if (registerCode is SystemErrorCode.ERROR_SUCCESS)
            _started = true;

        return RmFunctionReference.ErrorCodeToResult(
            registerCode,
            nameof(NativeMethods.RmRegisterResources)
        );
    }

    public Result<RmProcessInfo[]> GetProcesses(out RmRebootReason rebootReason)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_started)
            throw new InvalidOperationException(
                $"{nameof(RestartManagerSession)} has not been started."
            );

        const int attempts = 5;
        RmProcessInfo[]? buffer = null;
        SystemErrorCode code;
        rebootReason = RmRebootReason.None;

        for (int attempt = 0; attempt < attempts; attempt++)
        {
            uint count = (uint)(buffer?.Length ?? 0);

            code = NativeMethods.RmGetList(
                _sessionHandle,
                out uint needed,
                ref count,
                buffer,
                out rebootReason
            );

            switch (code)
            {
                case SystemErrorCode.ERROR_SUCCESS:
                    if (count == 0 || buffer is null)
                        return Result.Success<RmProcessInfo[]>([]);

                    var processes =
                        count == buffer.Length ? buffer : buffer.AsSpan(0, (int)count).ToArray();

                    return Result.Success(processes);

                case SystemErrorCode.ERROR_MORE_DATA:
                    buffer = new RmProcessInfo[needed + 4]; // adding some slack here avoids another round trip if a process opens the file
                    continue;

                default:
                    var result = RmFunctionReference.ErrorCodeToResult(
                        code,
                        nameof(NativeMethods.RmGetList)
                    );

                    return new Result<RmProcessInfo[]>(result.Errors, result.Type, []);
            }
        }

        return new Result<RmProcessInfo[]>(
            [
                $"{RmFunctionReference.ApiName} {nameof(NativeMethods.RmGetList)} kept returning {SystemErrorCode.ERROR_MORE_DATA} after {attempts}.",
            ],
            ResultType.InternalError,
            []
        );
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_started)
        {
            var code = NativeMethods.RmEndSession(_sessionHandle);

            if (code is not SystemErrorCode.ERROR_SUCCESS)
                Debug.WriteLine($"Failed to dispose session: {code}.");
        }

        _disposed = true;
    }

    private SystemErrorCode RegisterResourcesSafe(uint sessionHandle) =>
        NativeMethods.RmRegisterResources(
            sessionHandle,
            (uint)_filePaths.Length,
            _filePaths,
            0,
            null,
            0,
            null
        );
}
