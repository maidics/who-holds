using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.RestartManagerLibrary.Structs;

[InheritsTests]
internal sealed class RmUniqueProcessTests() : NativeStructTestBase<RmUniqueProcess>(12);
