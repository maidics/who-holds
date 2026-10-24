using System.Net.Sockets;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary;

internal static partial class IpHlpApi
{
    [LibraryImport(Libraries.IpHlpApi)] // returns the error code, does not set the last-error value
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode GetExtendedUdpTable(
        Span<byte> pUdpTable,
        ref uint pdwSize,
        [MarshalAs(UnmanagedType.Bool)] bool bOrder,
        AddressFamily ulAf,
        UdpTableClass tableClass,
        uint reserved
    );
}
