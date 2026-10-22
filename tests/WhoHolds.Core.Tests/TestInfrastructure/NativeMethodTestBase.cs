using System.Reflection;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.RestartManagerLibrary;

namespace WhoHolds.Core.Tests.TestInfrastructure;

[NotInParallel] // due to rm session handles: rm gives out handles starting from 0 so parallelizing these can lead to flaky tests
internal abstract class NativeMethodTestBase(Type library, string method)
{
    protected readonly MethodInfo _methodInfo = GetMethodInfo(library, method);

    private static MethodInfo GetMethodInfo(Type library, string method)
    {
        var m = library.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);

        if (m is null)
            throw new ArgumentException(
                $"{library.Name} internal static method not found: {method}."
            );

        return m;
    }

    protected ParameterInfo[] AssertMethodDeclaration(
        Type returnType,
        Type[] parameterTypes, // attributes on the parameters should not be tested since the compiler will ensure the correct ones are applied
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

    protected void ConsoleWriteFailedToEndRmSession(SystemErrorCode code)
    {
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(
            $"[cleanup] {nameof(RestartManager.RmEndSession)} failed: {code} ({(uint)code})."
        );
        Console.ForegroundColor = originalColor;
    }
}
