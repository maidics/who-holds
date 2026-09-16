using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NativeMethods;

internal sealed class NtQueryObjectTests()
    : NativeMethodTestBase(nameof(Core.Interop.NativeMethods.NtQueryObject))
{
    [Test]
    public override void ShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethodDeclaration(
            _methodInfo,
            typeof(NtStatus),
            [typeof(IntPtr), typeof(int), typeof(IntPtr), typeof(int), typeof(uint).MakeByRefType()],
            MethodAttributes.PinvokeImpl | MethodAttributes.Static
        );

        var returnLength = parameters[4];
        returnLength.IsIn.ShouldBeFalse();
        returnLength.IsOut.ShouldBeTrue();
    }

    [Test]
    public override void ShouldBeDecoratedWithImportAttribute()
    {
        var dllImport = _methodInfo.GetCustomAttribute<DllImportAttribute>();
        dllImport.ShouldNotBeNull();
        dllImport.Value.ShouldBe("ntdll.dll");
        dllImport.EntryPoint.ShouldBe("NtQueryObject");
    }
}
