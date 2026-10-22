using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class OpenProcessTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.OpenProcess))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        AssertMethodDeclaration(
            typeof(SafeProcessHandle),
            [typeof(ProcessAccessRights), typeof(bool), typeof(uint)],
            MethodAttributes.Static
        );
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var libraryImport = _methodInfo.GetCustomAttribute<LibraryImportAttribute>();
        libraryImport.ShouldNotBeNull();
        libraryImport.LibraryName.ShouldBe("kernel32.dll");
        libraryImport.SetLastError.ShouldBeTrue();

        var defaultDllImportSearchPaths =
            _methodInfo.GetCustomAttribute<DefaultDllImportSearchPathsAttribute>();
        defaultDllImportSearchPaths.ShouldNotBeNull();
        defaultDllImportSearchPaths.Paths.ShouldBe(DllImportSearchPath.System32);
    }
}
