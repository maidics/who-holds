using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop;

internal delegate NtStatus SystemQueryDelegate(
    SystemInformationClass cls,
    IntPtr buffer,
    int length,
    out int returnLength
);
