using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary;

internal static partial class IpHlpApi
{
    [LibraryImport(Libraries.IpHlpApi)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode GetOwnerModuleFromUdpEntry(
        in MibUdpRowOwnerModule pUdpEntry,
        TcpipOwnerModuleInfoClass infoClass,
        nint pBuffer,
        ref uint pdwSize
    );
}
