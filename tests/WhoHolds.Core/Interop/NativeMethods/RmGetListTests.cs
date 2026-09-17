using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;
using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class RmGetListTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.RmGetList))
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

        var startCode = Core.Interop.NativeMethods.RmStartSession(
            out uint pSessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(Core.Interop.NativeMethods.RmStartSession)
        );

        try
        {
            uint count = 1;

            var listCode = Core.Interop.NativeMethods.RmGetList(
                pSessionHandle,
                out _,
                ref count,
                null,
                out _
            );

            listCode.ShouldBe(SystemErrorCode.BadArguments);
        }
        finally
        {
            var endCode = Core.Interop.NativeMethods.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.Success)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }

    [Test]
    public void ShouldReturnInvalidHandle()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = Core.Interop.NativeMethods.RmStartSession(
            out uint pSessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(Core.Interop.NativeMethods.RmStartSession)
        );

        var endCode = Core.Interop.NativeMethods.RmEndSession(pSessionHandle);

        RestartManagerException.ThrowIfOperationFailed(
            endCode,
            nameof(Core.Interop.NativeMethods.RmEndSession)
        );

        uint count = 0;

        var listCode = Core.Interop.NativeMethods.RmGetList(
            pSessionHandle,
            out _,
            ref count,
            [],
            out _
        );
        listCode.ShouldBe(SystemErrorCode.InvalidHandle);
    }

    [Test]
    public void ShouldReturnSuccess()
    {
        Span<char> buffer = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];

        var startCode = Core.Interop.NativeMethods.RmStartSession(
            out uint pSessionHandle,
            0,
            buffer
        );

        RestartManagerException.ThrowIfOperationFailed(
            startCode,
            nameof(Core.Interop.NativeMethods.RmStartSession)
        );

        try
        {
            uint count = 0;

            var listCode = Core.Interop.NativeMethods.RmGetList(
                pSessionHandle,
                out _,
                ref count,
                [],
                out _
            );
            listCode.ShouldBe(SystemErrorCode.Success);
        }
        finally
        {
            var endCode = Core.Interop.NativeMethods.RmEndSession(pSessionHandle);

            if (endCode is not SystemErrorCode.Success)
                ConsoleWriteFailedToEndRmSession(endCode);
        }
    }
}
