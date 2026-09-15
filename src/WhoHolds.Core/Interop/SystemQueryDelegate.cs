using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop;

internal delegate NtStatus SystemQueryDelegate(
    SystemClass systemClass,
    IntPtr buffer,
    int length,
    out int returnLength
);
