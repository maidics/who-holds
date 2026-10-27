using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary;

internal static partial class IpHlpApi
{
    [LibraryImport(Libraries.IpHlpApi)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode GetOwnerModuleFromTcpEntry(
        in MibTcpRowOwnerModule pTcpEntry,
        TcpipOwnerModuleInfoClass infoClass,
        nint pBuffer,
        ref uint pdwSize
    );
}
