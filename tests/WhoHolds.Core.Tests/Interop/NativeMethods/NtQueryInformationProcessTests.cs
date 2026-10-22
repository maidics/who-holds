using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class NtQueryInformationProcessTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.NtQueryInformationProcess))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(NtStatus),
            [
                typeof(SafeProcessHandle),
                typeof(int),
                typeof(IntPtr),
                typeof(int),
                typeof(uint).MakeByRefType(),
            ],
            MethodAttributes.Static
        );

        parameters.Last().IsOut.ShouldBeTrue();
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var libraryImport = _methodInfo.GetCustomAttribute<LibraryImportAttribute>();
        libraryImport.ShouldNotBeNull();
        libraryImport.LibraryName.ShouldBe("ntdll.dll");
        libraryImport.SetLastError.ShouldBeFalse();

        var defaultDllImportSearchPaths =
            _methodInfo.GetCustomAttribute<DefaultDllImportSearchPathsAttribute>();
        defaultDllImportSearchPaths.ShouldNotBeNull();
        defaultDllImportSearchPaths.Paths.ShouldBe(DllImportSearchPath.System32);
    }
}
