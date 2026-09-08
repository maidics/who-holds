# Glossary

Term used throughout this project.

## P/Invoke - Platform Invoke

The .NET mechanism for calling functions in native (unmanaged) DLLs. Declared via
`[LibraryImport]` (modern, source-generated) or `[DllImport]` (legacy, runtime-generated).

## Interop - Interoperability

Code that allows managed (.NET) code to call unmanaged (native) code. In .NET, this lives under
the `System.Runtime.InteropServices` namespace and is implemented via [P/Invoke](#pinvoke-platform-invoke).

## Handle

An opaque reference number the OS gives a process to refer to a kernel object (a file, window,
process, mutex, etc.). Treated as opaque - the value of it is not interpreted, it is only passed be to the OS in subsequent calls.

## Marshalling

The process of converting data between managed (.NET) memory representations and unmanaged
(native) memory representations when crossing the [P/Invoke](#pinvoke---platform-invoke) boundary - e.g., converting a C#
`string` into a native UTF-16 buffer.

## Win32

The classic, documented, stable Windows API (`kernel32.dll`, `user32.dll`, etc.). Named for the 32-bit architecture era it originated in, this name persisted even after 64-bit Windows arrived.

## NT / Windows NT

The kernel architecture underlying all modern Windows versions. Also refers to the NT Native API - the undocumented/semi-documented API layer in `ntdll.dll` that [Win32](#win32) itself is built on top of.

## NtQueryObject

An NT native API function (`ntdll.dll`) that retrieves information about a given object handle,
such as its name (file path) or type. Used to resolve a raw handle into a human-readable path.

## NtQuerySystemInformation

An NT native API function (`ntdll.dll`) that retrieves various kinds of system-wide information.
