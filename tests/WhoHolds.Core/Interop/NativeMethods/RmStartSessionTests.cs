using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class RmStartSessionTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.RmStartSession))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(SystemErrorCode),
            [typeof(uint).MakeByRefType(), typeof(uint), typeof(Span<char>)],
            MethodAttributes.Static
        );

        var pSessionHandle = parameters[0];
        pSessionHandle.IsIn.ShouldBeFalse();
        pSessionHandle.IsOut.ShouldBeTrue();
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

    /* The following cases are not be tested here (handling of these errors are tested instead):
        - ERROR_OUTOFMEMORY: too expensive to set up for a highly unlikely case
        - ERROR_MAX_SESSIONS_REACHED: requires end session additionally: not ending the sessions could end up causing a reboot
        - ERROR_SEM_TIMEOUT: also too expensive to set up: have to find the mutex kernel object, but it is not documented
        - ERROR_WRITE_FAULT: requires admin rights to deny read and write to Registry, this not something I'd like to cover here either
       The tests here cover declaration instead of behaviour and error handling.
     */

    [Test]
    public void ShouldReturnBadArguments()
    {
        var code = Core.Interop.NativeMethods.RmStartSession(out _, 0, null!);
        code.ShouldBe(SystemErrorCode.ERROR_BAD_ARGUMENTS);
    }

    [Test]
    public void ShouldReturnSuccess()
    {
        Span<char> sessionKeyBuffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var code = Core.Interop.NativeMethods.RmStartSession(
            out uint pSessionHandle,
            0,
            sessionKeyBuffer
        );
        code.ShouldBe(SystemErrorCode.ERROR_SUCCESS);

        try
        {
            int end = sessionKeyBuffer.IndexOf('\0');
            end.ShouldBeGreaterThanOrEqualTo(0);

            string sessionKey = sessionKeyBuffer[..(end < 0 ? sessionKeyBuffer.Length : end)]
                .ToString();
            sessionKey.ShouldNotBeNullOrEmpty();
            sessionKey.ShouldNotContain('\0');
            sessionKey.Length.ShouldBe(RestartManagerLimits.CCH_RM_SESSION_KEY);
            Guid.TryParseExact(sessionKey, "N", out _).ShouldBeTrue();
        }
        finally
        {
            var endCode = Core.Interop.NativeMethods.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.ERROR_SUCCESS)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }
}
