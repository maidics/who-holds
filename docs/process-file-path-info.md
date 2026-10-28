# Process File Path Information

## 1. Open process & confirm it is the same one
- Process IDs are reused by Windows so this is required:
  - Call `OpenProcess(...)` and compare start times (via `GetProcessTimes(...)`)
  - If differs return a failure: `ImagePath` stays `null` and the reason is added to `Result.Errors`
- It makes sense to directly query this via `OpenProcess(...)` which allows `PROCESS_DUP_HANDLE` (_process_ handle right access):
  - It copies a reference to a _process handle_ into the program's handle table so it can be used to query the process's handles.
  - This is required because **handle values only mean something inside the process's own handle table**.

## 2. Use `QueryFullProcessImageNameW` to resolve file path to the process
- The function requires a buffer which is set to the max path length possible (32,767 characters).

---

_RM does handle loaded DLLs and mapped files: the following is needed for these special cases._

## 3. Query loaded DLLs and memory-mapped files (not implemented)
- The above lock files without any open handle
- `EnumProcessModulesEx` + `GetModuleFileNameExW` (psapi) covers images.
- Walking `VirtualQueryEx` and calling `GetMappedFileNameW` on `MEM_MAPPED` regions covers data mappings 
  => returns `\Device\HarddiskVolumeN\...` paths => map to drive letters via `QueryDosDevice`

---

## Limits
- **Privileges**: requires elevated privileges for other-user, service processes with admin requirement, protected processes (antivirus etc.).
  - PROCESS_DUP_HANDLE refuses these cases - how to guard against this?
- **Snapshot**: the holder process can exit between `RmGetList` and step 1, making `OpenProcess` fail. It is reported in `Result.Errors`, the holder is still returned with a `null` `ImagePath`.