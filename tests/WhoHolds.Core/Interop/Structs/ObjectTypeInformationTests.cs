using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

[InheritsTests]
internal sealed class ObjectTypeInformationTests : NtStructTestBase<ObjectTypeInformation>
{
    [Test]
    public override void ShouldHaveCorrectSize()
    {
        AssertSize(104);
    }
}
