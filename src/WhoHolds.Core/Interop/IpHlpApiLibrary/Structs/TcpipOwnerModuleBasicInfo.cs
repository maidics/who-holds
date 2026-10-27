using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

// all native struct variants are the same: TCP, UDP, v4 and v6
[StructLayout(LayoutKind.Sequential)]
internal readonly struct TcpipOwnerModuleBasicInfo : INativeSized<TcpipOwnerModuleBasicInfo>
{
    public readonly nint pModuleName;
    public readonly nint pModulePath;

    public string? ModuleName => Marshal.PtrToStringUni(pModuleName);
    public string? ModulePath => Marshal.PtrToStringUni(pModulePath);

    public static int Size => Marshal.SizeOf<TcpipOwnerModuleBasicInfo>();
}
