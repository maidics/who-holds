using WhoHolds.Core.Interop.NtDllLibrary.Enums;
using WhoHolds.Core.Interop.NtDllLibrary.Structs;

namespace WhoHolds.Core.Interop.NtDllLibrary.Delegates;

internal delegate NtStatus SystemQueryDelegate(
    SystemClass systemClass,
    IntPtr buffer,
    int length,
    out uint returnLength
);
