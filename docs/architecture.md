# Architecture

## Layers
| Layer                                              | Project                                | Owns                                   | Must not leak past it                                     |
|------------------------------------|----------------------------------------|----------------------------------------|-----------------------------------------------------------|
| NativeMethods                       | [WhoHolds.Core](../src/WhoHolds.Core/) | raw signatures                         | nothing; no logic lives here                              |
| Native method handlers              | [WhoHolds.Core](../src/WhoHolds.Core/) | handle lifetime, call order            | uint handles, Win32 error codes (map to enums/ constants) |
| Orchestrator classes                                        | [WhoHolds.Core](../src/WhoHolds.Core/) | domain results              | RM_PROCESS_INFO, anything Win32                           |
| Presentation: CLI         | [WhoHolds.Cli](../src/WhoHolds.Cli/)   | Commands                                | -                                       

### [**Interop**](../src/WhoHolds.Core/Interop/)

Native API related definitions should be marked as internal and it should not be accessible to [WhoHolds.Cli](../src/WhoHolds.Cli/).