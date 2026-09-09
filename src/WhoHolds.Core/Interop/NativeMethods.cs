using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop;

internal static class NativeMethods
{
    private const string NtDll = "ntdll.dll";

    [DllImport(NtDll, EntryPoint = "NtQuerySystemInformation")]
    internal static extern uint GetNtSystemInfo(int systemInformationClass, IntPtr systemInformation, int systemInformationLength, out int returnLength);
}
