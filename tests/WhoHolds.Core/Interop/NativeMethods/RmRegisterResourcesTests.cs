using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class RmRegisterResourcesTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.RmRegisterResources))
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

        var startCode = Core.Interop.NativeMethods.RmStartSession(
            out uint sessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(Core.Interop.NativeMethods.RmStartSession)
        );

        try
        {
            var code = Core.Interop.NativeMethods.RmRegisterResources(
                sessionHandle,
                1,
                null,
                0,
                null,
                0,
                []
            );
            code.ShouldBe(SystemErrorCode.BadArguments);
        }
        finally
        {
            var endCode = Core.Interop.NativeMethods.RmEndSession(sessionHandle);

            if (endCode is not SystemErrorCode.Success)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }

    // TODO: add additional tests for RmRegisterResources & RmGetList
}
