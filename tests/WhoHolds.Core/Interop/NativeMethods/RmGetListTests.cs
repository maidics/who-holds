using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
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
}
