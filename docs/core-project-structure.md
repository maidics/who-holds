# Core Project Structure

```
Interop/
├── NativeMethods.cs        # NtQuerySystemInformation, NtQueryObject, DuplicateHandle, NtClose
├── Structs/
│   ├── SystemHandleInfo.cs # mirrors the native SYSTEM_HANDLE_INFORMATION layout
│   └── ObjectNameInfo.cs
└── NtStatus.cs             # NTSTATUS result codes as an enum

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

## `HandleFinder.cs`

The main public entrypoint - `HandleFinder.WhoHolds(path)`.

## `Internal/`

Implementations that support [`HandleFinder`](#handlefindercs) but are not public API. All content in this folder should be marked as internal.

## `Abstractions/`

Interfaces, if [`HandleFinder`](#handlefindercs) needs to be mockable so tests can run without real OS calls. Only worth it if tests actually demand it.