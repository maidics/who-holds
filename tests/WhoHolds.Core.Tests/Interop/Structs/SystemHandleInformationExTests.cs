using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

[InheritsTests]
internal sealed class SystemHandleInformationExTests()
    : NativeStructTestBase<SystemHandleEntryEx>(40);
