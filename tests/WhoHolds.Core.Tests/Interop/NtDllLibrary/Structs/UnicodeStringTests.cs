using WhoHolds.Core.Interop.NtDllLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.NtDllLibrary.Structs;

[InheritsTests]
internal sealed class UnicodeStringTests() : NativeStructTestBase<UnicodeString>(16);
