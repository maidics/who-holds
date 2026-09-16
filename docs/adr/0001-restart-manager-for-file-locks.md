# ADR-0001: Use the Windows Restart Manager for file-lock detection

**Status:** Accepted

## Context

The UI names the process holding a file when an operation fails with `ERROR_SHARING_VIOLATION` (32), in place of the bare Win32 message.

The prototype path enumerates system handles through [`NtQuerySystemInformation`](https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation) with `SystemExtendedHandleInformation`, resolves each file handle's path, and matches it against the target. Four failures:

- The API is undocumented. Info-class numbering and struct layouts shift across Windows builds, forcing version-conditional marshalling.
- Full results require `SeDebugPrivilege`. Standard-user runs degrade silently.
- [`NtQueryObject`](https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryobject) with `ObjectNameInformation` blocks indefinitely on synchronous named pipes and certain device objects. The mitigation is an uncancellable worker thread with a timeout.
- Cost is O(all handles on the machine), routinely 100k+, to resolve one path.

## Decision

Replace handle enumeration with the Windows Restart Manager (`rstrtmgr.dll`): [`RmStartSession`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmstartsession) → [`RmRegisterResources`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmregisterresources) → [`RmGetList`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmgetlist) → [`RmEndSession`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmendsession).

## Options Considered

### Option A: Handle enumeration via [`NtQuerySystemInformation`](https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntquerysysteminformation) (prototype)

| Dimension | Assessment |
|---|---|
| Complexity | High - version-conditional structs, handle duplication, timeout threads |
| Runtime cost | High - full handle scan per query |
| Privileges | `SeDebugPrivilege` for full results |
| Supportability | Poor - undocumented, no compatibility guarantee |
| Coverage | Widest - files, directories, any named object |

**Pros:** Only option covering directory handles and arbitrary kernel objects.
**Cons:** Undocumented, elevation-gated, hangs on pipe handles, full scan per query.

### Option B: Restart Manager (chosen)

| Dimension | Assessment |
|---|---|
| Complexity | Low - four exported functions |
| Runtime cost | Low - sub-second for a handful of registered paths |
| Privileges | None for same-user processes |
| Supportability | Good - documented since Vista, stable error codes |
| Coverage | Files only, best-effort across security boundaries |

**Pros:** Documented and stable, and returns application name, application type, terminal-services session id and restartable flag rather than a bare PID.
**Cons:** Files only, silent omission across security boundaries, registry-backed registration cost, 64-session cap.

### Option C: [`NtQueryInformationFile`](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntqueryinformationfile) with `FileProcessIdsUsingFileInformation` (unevaluated)

| Dimension | Assessment |
|---|---|
| Complexity | Low - one call against an open handle |
| Runtime cost | Very low - no enumeration |
| Privileges | None beyond opening the file |
| Supportability | Poor - undocumented |
| Coverage | Files only, bare PIDs |

**Pros:** Lowest cost per query, and precise.
**Cons:** Retains an undocumented dependency, requires a handle to the target first, and returns PIDs without application or service metadata.

Assessment drawn from documentation. No measurement taken.

## Trade-off Analysis

Axis: supportability against coverage. Supportability wins. Cost: directories stay uncovered and empty results become ambiguous.

Option A's coverage advantage is gated on elevation, so the standard-user case receives the same partial answer as Option B while paying for a full handle scan and carrying the hang risk. Option C keeps the undocumented dependency that motivates this record, and drops the metadata that makes the error message readable.

An empty result now means "no locker visible", not "no locker exists". The UI must state the difference.

## Consequences

**What becomes easier**

- Startup drops the `SeDebugPrivilege` request and the elevation hint, so behaviour is identical for standard users.
- Error messages name the application directly, for example "Excel (PID 4821)".
- Documented error codes replace best-guess recovery.
- Maintenance no longer tracks undocumented struct layouts across Windows builds.

**What becomes harder**

- Every call site must treat "no lockers found" as inconclusive, and UI copy must say so.
- [`RmGetList`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmgetlist) returns `ERROR_MORE_DATA` (234) on an undersized buffer, and the locker set can change between sizing and fetching, so the call needs a bounded retry loop.
- Sessions are capped at 64 per user session, so each session must be short-lived and ended in a `finally` or via `Dispose`. A leak degrades the whole login session.
- [`RmRegisterResources`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmregisterresources) performs registry writes, so paths batch into one call. Per-file registration across a directory tree matches the [documented Conti ransomware pattern](https://www.crowdstrike.com/en-us/blog/windows-restart-manager-part-2/) and stays out of this codebase.

**What triggers a revisit**

- Directories. Folder handles and the current-working-directory case are uncovered. Supported folder deletion reopens Option A behind a feature flag, or a library wrapping the same API.
- Ports. Permanently outside the Restart Manager, which registers files, services and processes. Port ownership belongs to [`GetExtendedTcpTable`](https://learn.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getextendedtcptable) with [`TCP_TABLE_OWNER_PID_ALL`](https://learn.microsoft.com/en-us/windows/win32/api/iprtrmib/ne-iprtrmib-tcp_table_class) and needs its own record.
- Shutdown integration. [`RmShutdown`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmshutdown) and [`RmRestart`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmrestart) stay unused here. Terminating an editor is a different action from naming it and needs its own record.

## Notes

The Restart Manager was unknown when the prototype was written. Option A was an oversight, not a rejection of Option B.