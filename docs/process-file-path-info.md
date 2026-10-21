# Process File Path Information

## 1. Open process & confirm it is the same one
- Process IDs are reused by Windows so this is required:
  - Call `OpenProcess(...)` and compare start times
  - If differs return `null`
- It makes sense to directly query this via `OpenProcess(...)` which allows `PROCESS_DUP_HANDLE` (_process_ handle right access):
  - It copies a reference to a _process handle_ so we can query the process's handles.
  - This is required because **handle values only mean something inside the process's own handle table**.
  - After this `GetFinalPathNameByHandle(myHandle)` works because there's a reference in the handle table.

## 2. Query process handles
- P/Invoke `NtQueryInformationProcess` using the process handle reference to query its handles.
- Keep entries that is of `File` type.

## 3. Resolve each handle to a file path
- Duplicate handles to own process: values from the process's handle table only makes sense to the process.
- Call `GetFileType`, skip anything that is not `FILE_TYPE_DISK` (drop pipes and devices).
- Call `GetFinalPathNameByHandleW(..., VOLUME_NAME_DOS)` gives `C:\...` - strip `\\?\` prefix.
- Close duplicated handles.
- Synchronous disk handle with a blocking I/O can hand the query - keep a timeout

---

This will resolve everything that is in a loaded DLL or a mapped file.

_RM do handle loaded DLLs and mapped files: the following is needed for these special cases._

## 4. Query loaded DLLs and memory-mapped files
- The above lock files without any open handle
- `EnumProcessModulesEx` + `GetModuleFileNameExW` (psapi) covers images.
- Walking `VirtualQueryEx` and calling `GetMappedFileNameW` on `MEM_MAPPED` regions covers data mappings 
  => returns `\Device\HarddiskVolumeN\...` paths => map to drive letters via `QueryDosDevice`

---

## Limits
- **Privileges**: requires elevated privileges for other-user, service processes with admin requirement, protected processes (antivirus etc.).
  - PROCESS_DUP_HANDLE refuses these cases - how to guard against this?
- **Snapshot**: handles can close between step 2 and 3 returning `ERROR_INVALID_HANDLE`. Treat it normal, not a failure.