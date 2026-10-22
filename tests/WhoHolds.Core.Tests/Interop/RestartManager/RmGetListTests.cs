using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class RmGetListTests()
    : NativeMethodTestBase(typeof(RestartManager), nameof(RestartManager.RmGetList))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(SystemErrorCode),
            [
                typeof(uint),
                typeof(uint).MakeByRefType(),
                typeof(uint).MakeByRefType(),
                typeof(RmProcessInfo[]),
                typeof(RmRebootReason).MakeByRefType(),
            ],
            MethodAttributes.Static
        );

        var pnProcInfoNeeded = parameters[1];
        pnProcInfoNeeded.IsIn.ShouldBeFalse();
        pnProcInfoNeeded.IsOut.ShouldBeTrue();

        var pnProcInfo = parameters[2];
        pnProcInfo.ParameterType.IsByRef.ShouldBeTrue(); // ref uint pnProcInfo

        var pnRebootReason = parameters[4];
        pnRebootReason.IsIn.ShouldBeFalse();
        pnRebootReason.IsOut.ShouldBeTrue();
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var libraryImport = _methodInfo.GetCustomAttribute<LibraryImportAttribute>();
        libraryImport.ShouldNotBeNull();
        libraryImport.LibraryName.ShouldBe("rstrtmgr.dll");
        libraryImport.StringMarshalling.ShouldBe(StringMarshalling.Utf16);
    }

    [Test]
    public void ShouldBeDecoratedWithDefaultDllImportSearchPathsAttribute()
    {
        var defaultDllImportSearchPaths =
            _methodInfo.GetCustomAttribute<DefaultDllImportSearchPathsAttribute>();
        defaultDllImportSearchPaths.ShouldNotBeNull();
        defaultDllImportSearchPaths.Paths.ShouldBe(DllImportSearchPath.System32);
    }

    [Test]
    public unsafe void ShouldReturnBadArguments()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = RestartManager.RmStartSession(
            out uint pSessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(RestartManager.RmStartSession)
        );

        try
        {
            uint count = 1;

            var listCode = RestartManager.RmGetList(
                pSessionHandle,
                out _,
                ref count,
                null,
                out _
            );

            listCode.ShouldBe(SystemErrorCode.ERROR_BAD_ARGUMENTS);
        }
        finally
        {
            var endCode = RestartManager.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.ERROR_SUCCESS)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }

    [Test]
    public void ShouldReturnInvalidHandle()
    {
        uint count = 0;

        var listCode = RestartManager.RmGetList(
            uint.MaxValue,
            out _,
            ref count,
            [],
            out _
        );

        listCode.ShouldBe(SystemErrorCode.ERROR_INVALID_HANDLE);
    }

    [Test]
    public void ShouldReturnSuccess()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = RestartManager.RmStartSession(
            out uint pSessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(RestartManager.RmStartSession)
        );

        try
        {
            uint count = 0;

            var listCode = RestartManager.RmGetList(
                pSessionHandle,
                out _,
                ref count,
                [],
                out _
            );
            listCode.ShouldBe(SystemErrorCode.ERROR_SUCCESS);
        }
        finally
        {
            var endCode = RestartManager.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.ERROR_SUCCESS)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }
}
