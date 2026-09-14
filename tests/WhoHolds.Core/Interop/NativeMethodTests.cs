using System.Reflection;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class NativeMethodTests
{
    private MethodInfo? GetMethodInfo(string method) =>
        typeof(NativeMethods).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static); // asserts that it is internal

    private ParameterInfo[] AssertMethod(
        string method,
        Type returnType,
        Type[] expectedParameterTypes,
        string entryPoint
    )
    {
        var methodInfo = GetMethodInfo(method);
        methodInfo.ShouldNotBeNull();
        methodInfo.ReturnType.ShouldBe(returnType);
        methodInfo.Attributes.HasFlag(MethodAttributes.Static).ShouldBeTrue();
        methodInfo.Attributes.HasFlag(MethodAttributes.PinvokeImpl).ShouldBeTrue();

        var parameters = methodInfo.GetParameters();

        parameters.Select(p => p.ParameterType).ShouldBe(expectedParameterTypes);

        var dllImport = methodInfo.GetCustomAttribute<DllImportAttribute>();
        dllImport.ShouldNotBeNull();
        dllImport.Value.ShouldBe("ntdll.dll");
        dllImport.EntryPoint.ShouldBe(entryPoint);

        return parameters;
    }

    [Test]
    public void NtQuerySystemInfoShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethod(
            nameof(NativeMethods.NtQuerySystemInfo),
            typeof(NtStatus),
            [
                typeof(SystemInformationClass),
                typeof(IntPtr),
                typeof(int),
                typeof(int).MakeByRefType(),
            ],
            "NtQuerySystemInformation"
        );

        var ret = parameters[3];
        ret.IsOut.ShouldBeTrue();
        ret.IsIn.ShouldBeFalse();
    }

    [Test]
    public void NtQueryObjectShouldBeDeclaredCorrectly()
    {
        var parameters = AssertMethod(
            nameof(NativeMethods.NtQueryObject),
            typeof(NtStatus),
            [
                typeof(IntPtr),
                typeof(SystemInformationClass),
                typeof(IntPtr),
                typeof(int),
                typeof(int).MakeByRefType(),
            ],
            "NtQueryObject"
        );

        var ret = parameters[4];
        ret.IsOut.ShouldBeTrue();
        ret.IsIn.ShouldBeFalse();
    }
}
