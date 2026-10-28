using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary;

[InheritsTests]
internal sealed class GetExtendedUdpTableTests()
    : NativeMethodTestBase(typeof(IpHlpApi), nameof(IpHlpApi.GetExtendedUdpTable))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            typeof(SystemErrorCode),
            [
                typeof(Span<byte>),
                typeof(uint).MakeByRefType(),
                typeof(bool),
                typeof(AddressFamily),
                typeof(UdpTableClass),
                typeof(uint),
            ],
            MethodAttributes.Static
        );

        parameters[1].ParameterType.IsByRef.ShouldBeTrue();
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
