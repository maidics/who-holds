using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

[InheritsTests]
internal sealed class QueryFullProcessImageNameWTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.QueryFullProcessImageNameW))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(bool),
            [
                typeof(SafeProcessHandle),
                typeof(uint),
                typeof(Span<char>),
                typeof(uint).MakeByRefType(),
            ],
            MethodAttributes.Static
        );

        parameters.Last().ParameterType.IsByRef.ShouldBeTrue();
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
