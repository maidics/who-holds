using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class GetProcessTimesTests()
    : NativeMethodTestBase(typeof(Kernel32), nameof(Kernel32.GetProcessTimes))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(bool),
            [
                typeof(SafeProcessHandle),
                typeof(FileTime).MakeByRefType(),
                typeof(FileTime).MakeByRefType(),
                typeof(FileTime).MakeByRefType(),
                typeof(FileTime).MakeByRefType(),
            ],
            MethodAttributes.Static
        );

        parameters.Skip(1).ShouldAllBe(p => p.IsOut);
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
