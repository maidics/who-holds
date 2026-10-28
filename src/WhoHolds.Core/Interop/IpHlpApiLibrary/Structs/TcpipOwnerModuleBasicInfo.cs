using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

// all native struct variants are the same: TCP, UDP, v4 and v6
[StructLayout(LayoutKind.Sequential)]
internal readonly struct TcpipOwnerModuleBasicInfo
{
    public readonly nint pModuleName;
    public readonly nint pModulePath;

    public string? ModuleName => Marshal.PtrToStringUni(pModuleName);
    public string? ModulePath => Marshal.PtrToStringUni(pModulePath);
}
