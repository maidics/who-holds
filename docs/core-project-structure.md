# Core Project Structure

```
WhoHolds.Core/
├── Common/
│   ├── Enums/                           # public enums: PortProtocol, IpVersion
│   └── Models/                          # public models: Result, FileHolderProcess, PortHolderProcess
├── Interop/
├── Utility/                             # helpers: PathUtils, ByteFormat
├── WhoHolds.cs
└── WhoHolds.Core.csproj

Interop/
├── Libraries.cs                         # native library names: Libraries.Kernel32, Libraries.RestartManager, Libraries.IpHlpApi
├── Common/                              # interop concerns shared across libraries: FileTime, SystemErrorCode, NativeBuffer
├── Kernel32Library/
│   ├── Enums/                           # structs, enums, constants and interfaces belonging to the library, grouped by kind
│   │   └── ProcessAccessRights.cs
│   └── Kernel32.OpenProcess.cs          # one P/Invoke declaration per file on the static partial Kernel32 class
├── RestartManagerLibrary/
│   ├── Constants/
│   ├── Enums/
│   ├── Structs/
│   │   └── RmProcessInfo.cs
│   └── RestartManager.RmStartSession.cs
├── IpHlpApiLibrary/
│   ├── Constants/
│   ├── Enums/
│   ├── Interfaces/
│   ├── Structs/
│   │   └── MibTcpRowOwnerModule.cs
│   └── IpHlpApi.GetExtendedTcpTable.cs
└── Services/                            # classes calling the declarations: RestartManagerSession, ProcessPathResolver, PortOwnerModuleTableQuery, ...
```

## `WhoHolds.cs`

The main public entrypoint - `WhoHolds.File(path)` and `WhoHolds.Port(port, ipVersion, portProtocol)`. Orchestrates the [`Services`](#interop) and maps their output to the public [`Common`](#common) models.

## `Common/`

Public API types returned by or passed to [`WhoHolds`](#whoholdscs). Namespaces follow the folders (`WhoHolds.Core.Common.Enums`, `WhoHolds.Core.Common.Models`).

## `Interop/`

Vertically sliced by native library. A library folder is named `<Library>Library` and holds a `static partial` class named after the library, split into one `<Library>.<Method>.cs` file per declaration at the folder root, plus the native types only that library uses, grouped by kind into `Structs/`, `Enums/`, `Constants/` and `Interfaces/`. Types used by more than one library go into `Common/`, and classes that call the declarations go into `Services/`.

The `Library` suffix on the folder keeps the namespace from clashing with the class: namespaces follow the folders (`WhoHolds.Core.Interop.RestartManagerLibrary`, `WhoHolds.Core.Interop.RestartManagerLibrary.Structs`) while the class stays `RestartManager`. `Common/` and `Services/` follow the folder namespaces as well (`WhoHolds.Core.Interop.Common`, `WhoHolds.Core.Interop.Services`).

## `Utility/`

Helpers that are not tied to a native library.
