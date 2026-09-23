using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class RmEndSessionTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.RmEndSession))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        AssertMethodDeclaration(typeof(SystemErrorCode), [typeof(uint)], MethodAttributes.Static);
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var libraryImport = _methodInfo.GetCustomAttribute<LibraryImportAttribute>();
        libraryImport.ShouldNotBeNull();
        libraryImport.LibraryName.ShouldBe("rstrtmgr.dll");
    }

    [Test]
    public void ShouldBeDecoratedWithDefaultDllImportSearchPaths()
    {
        var defaultDllImportSearchPaths =
            _methodInfo.GetCustomAttribute<DefaultDllImportSearchPathsAttribute>();
        defaultDllImportSearchPaths.ShouldNotBeNull();
        defaultDllImportSearchPaths.Paths.ShouldBe(DllImportSearchPath.System32);
    }

    // Similarly to the RmStartSessionTests this class does not cover error handling only declaration

    [Test]
    public void ShouldReturnInvalidHandle()
    {
        var code = Core.Interop.NativeMethods.RmEndSession(uint.MaxValue); // rm session handles start from 0 so this is safe
        code.ShouldBe(SystemErrorCode.ERROR_INVALID_HANDLE);
    }
}
