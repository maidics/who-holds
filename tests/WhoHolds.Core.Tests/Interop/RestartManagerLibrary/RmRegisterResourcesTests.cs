using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.RestartManagerLibrary;
using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;
using WhoHolds.Core.Interop.RestartManagerLibrary.Exceptions;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.RestartManagerLibrary;

internal sealed class RmRegisterResourcesTests()
    : NativeMethodTestBase(typeof(RestartManager), nameof(RestartManager.RmRegisterResources))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        AssertMethodDeclaration(
            typeof(SystemErrorCode),
            [
                typeof(uint),
                typeof(uint),
                typeof(string[]) /* nullable reference types are compile time annotations only */
                ,
                typeof(uint),
                typeof(RmUniqueProcess[]),
                typeof(uint),
                typeof(string[]),
            ],
            MethodAttributes.Static
        );
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

    // only error codes that imply declaration correctness are tested here; same as RmStartSessionTests

    [Test]
    public void ShouldReturnBadArguments()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = RestartManager.RmStartSession(out uint sessionHandle, 0, buffer);

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(RestartManager.RmStartSession)
        );

        try
        {
            var code = RestartManager.RmRegisterResources(sessionHandle, 1, null, 0, null, 0, []);
            ShouldBeTestExtensions.ShouldBe(code, SystemErrorCode.ERROR_BAD_ARGUMENTS);
        }
        finally
        {
            var endCode = RestartManager.RmEndSession(sessionHandle);

            if (endCode is not SystemErrorCode.ERROR_SUCCESS)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }

    [Test]
    public void ShouldReturnInvalidHandle()
    {
        var code = RestartManager.RmRegisterResources(uint.MaxValue, 0, null, 0, null, 0, null);

        ShouldBeTestExtensions.ShouldBe(code, SystemErrorCode.ERROR_INVALID_HANDLE);
    }

    [Test]
    public void ShouldReturnSuccess()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = RestartManager.RmStartSession(out uint pSessionHandle, 0, buffer);

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(RestartManager.RmStartSession)
        );

        try
        {
            var registerCode = RestartManager.RmRegisterResources(
                pSessionHandle,
                0,
                null,
                0,
                null,
                0,
                null
            );

            ShouldBeTestExtensions.ShouldBe(registerCode, SystemErrorCode.ERROR_SUCCESS);
        }
        finally
        {
            var endCode = RestartManager.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.ERROR_SUCCESS)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }
}
