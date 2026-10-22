# Core Project Structure

```
Interop/
├── Libraries.cs                         # native library names: Libraries.Kernel32, Libraries.NtDll, Libraries.RestartManager
├── Common/                              # interop concerns shared across libraries: INativeSized, FileTime, SystemErrorCode, NativeBuffer
├── Kernel32/
│   ├── Kernel32.OpenProcess.cs          # one P/Invoke declaration per file on the static partial Kernel32 class
│   └── ProcessAccessRights.cs           # structs, enums, constants and exceptions belonging to the library
├── NtDll/
│   ├── NtDll.NtQuerySystemInfo.cs
│   └── SystemHandleEntryEx.cs
├── RestartManager/
│   ├── RestartManager.RmStartSession.cs
│   └── RmProcessInfo.cs
└── Services/                            # classes calling the declarations: RestartManagerSession, SystemQuery, ObjectTypes, ProcessPathResolver

WhoHolds.Core/
├── Interop/
├── Models/
├── Exceptions/
├── Extensions/
├── Internal/
├── Constants/
├── Abstractions/
├── Utilities/
├── HandleFinder.cs
└── WhoHolds.Core.csproj
```

## `Interop/`

Vertically sliced by native library. A library folder holds a `static partial` class named after the library, split into one `<Library>.<Method>.cs` file per declaration, plus the native types only that library uses. Types used by more than one library go into `Common/`, and classes that call the declarations go into `Services/`.

Library folders do not get their own namespace: `WhoHolds.Core.Interop.RestartManager` would clash with the `RestartManager` class, so their content lives in `WhoHolds.Core.Interop`. `Common/` and `Services/` follow the folder namespaces (`WhoHolds.Core.Interop.Common`, `WhoHolds.Core.Interop.Services`).

## `HandleFinder.cs`

The main public entrypoint - `HandleFinder.WhoHolds(path)`.

## `Internal/`

Implementations that support [`HandleFinder`](#handlefindercs) but are not public API. All content in this folder should be marked as internal.

## `Abstractions/`

Interfaces, if [`HandleFinder`](#handlefindercs) needs to be mockable so tests can run without real OS calls. Only worth it if tests actually demand it.