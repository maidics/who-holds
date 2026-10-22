using WhoHolds.Core.Interop;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop;

[InheritsTests]
internal sealed class SystemHandleInformationExTests()
    : NativeStructTestBase<SystemHandleEntryEx>(40);
