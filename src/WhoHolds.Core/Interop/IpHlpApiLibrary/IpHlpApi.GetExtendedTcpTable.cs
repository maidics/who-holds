using System.Net.Sockets;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary;

internal static partial class IpHlpApi
{
    [LibraryImport(Libraries.IpHlpApi)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode GetExtendedTcpTable(
        Span<byte> pTcpTable,
        ref uint pdwSize,
        [MarshalAs(UnmanagedType.Bool)] bool bOrder,
        AddressFamily ulAf,
        TcpTableClass tableClass,
        uint reserved
    );
}
