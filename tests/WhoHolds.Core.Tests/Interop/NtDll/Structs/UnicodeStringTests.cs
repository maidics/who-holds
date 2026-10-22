using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

[InheritsTests]
internal sealed class UnicodeStringTests() : NativeStructTestBase<UnicodeString>(16);
