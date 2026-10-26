namespace WhoHolds.Core.Interop;

/// <summary>
/// Names of the native libraries behind the raw P/Invoke declarations for the Windows API:
/// <c>kernel32.dll</c>, <c>rstrtmgr.dll</c>.
/// </summary>
/// <remarks>
/// <para>
/// Each library has its own folder holding a <c>static partial</c> class named after it, with one
/// declaration per file (<c>RestartManager/RestartManager.RmStartSession.cs</c>).
/// </para>
/// <para>
/// Signatures only. Members mirror the native declarations one-to-one, including naming and capitalization, per the .NET interop guidance.
/// </para>
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices">
/// Native interoperability best practices
/// </seealso>
internal static class Libraries
{
    internal const string Kernel32 = "kernel32.dll";
    internal const string RestartManager = "rstrtmgr.dll";
}
