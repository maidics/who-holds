using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary;

[InheritsTests]
internal sealed class GetOwnerModuleFromTcpEntryTests()
    : NativeMethodTestBase(typeof(IpHlpApi), nameof(IpHlpApi.GetOwnerModuleFromTcpEntry))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(SystemErrorCode),
            [
                typeof(MibTcpRowOwnerModule).MakeByRefType(),
                typeof(TcpipOwnerModuleInfoClass),
                typeof(nint),
                typeof(uint).MakeByRefType(),
            ],
            MethodAttributes.Static
        );

        parameters[0].IsIn.ShouldBe(true);
        parameters[0].IsOut.ShouldBeFalse();

        parameters[3].ParameterType.IsByRef.ShouldBeTrue();
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var libraryImport = _methodInfo.GetCustomAttribute<LibraryImportAttribute>();
        libraryImport.ShouldNotBeNull();
        libraryImport.LibraryName.ShouldBe(Libraries.IpHlpApi);

        var defaultDllImportPaths =
            _methodInfo.GetCustomAttribute<DefaultDllImportSearchPathsAttribute>();
        defaultDllImportPaths.ShouldNotBeNull();
        defaultDllImportPaths.Paths.ShouldBe(DllImportSearchPath.System32);
    }
}
