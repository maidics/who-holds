namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class Testing
{
    private const string EnvVarName = "DOTNET_ENVIRONMENT";
    private static string? _original;

    [Before(Assembly)]
    public static void SetUp()
    {
        _original = Environment.GetEnvironmentVariable(EnvVarName);
        Environment.SetEnvironmentVariable(EnvVarName, "Testing");
    }

    [After(Assembly)]
    public static void TearDown()
    {
        Environment.SetEnvironmentVariable(EnvVarName, _original); // if it was null then it removes the env var
    }
}
