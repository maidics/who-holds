using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

[InheritsTests]
internal sealed class RmUniqueProcessTests() : NativeStructTestBase<RmUniqueProcess>(12);
