using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class RmEndSessionTests()
    : NativeMethodTestBase(typeof(RestartManager), nameof(RestartManager.RmEndSession))
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
        var code = RestartManager.RmEndSession(uint.MaxValue); // rm session handles start from 0 so this is safe
        code.ShouldBe(SystemErrorCode.ERROR_INVALID_HANDLE);
    }
}
