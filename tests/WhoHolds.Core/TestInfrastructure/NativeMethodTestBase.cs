using System.Reflection;
using WhoHolds.Core.Interop;

namespace WhoHolds.Core.Tests.TestInfrastructure;

internal abstract class NativeMethodTestBase(string method)
{
    protected readonly MethodInfo _methodInfo = GetMethodInfo(method);

    private static MethodInfo GetMethodInfo(string method)
    {
        var m = typeof(NativeMethods).GetMethod(
            method,
            BindingFlags.NonPublic | BindingFlags.Static
        );

        if (m is null)
            throw new ArgumentException(
                $"{nameof(NativeMethods)} internal static method not found: {method}."
            );

        return m;
    }

    protected ParameterInfo[] AssertMethodDeclaration(
        Type returnType,
        Type[] parameterTypes,
        MethodAttributes methodAttribute
    )
    {
        _methodInfo.ReturnType.ShouldBe(returnType);

        _methodInfo.Attributes.HasFlag(methodAttribute).ShouldBeTrue();

        var parameters = _methodInfo.GetParameters();
        parameters.Select(p => p.ParameterType).ShouldBe(parameterTypes);

        return parameters;
    }

    public abstract void ShouldBeDeclaredCorrectly();

    public abstract void ShouldBeDecoratedWithImportAttribute();
}
