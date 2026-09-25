using System.Runtime.CompilerServices;

namespace WhoHolds.Pipeline.Tests;

internal static class Testing
{
    // this is set when a type is loaded from this Assembly
    // because a child process cannot change the parent's env this will die with the testing process
    [ModuleInitializer]
    internal static void Initialize() =>
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
}
