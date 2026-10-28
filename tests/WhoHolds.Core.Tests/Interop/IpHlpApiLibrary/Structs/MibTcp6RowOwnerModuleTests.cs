using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class MibTcp6RowOwnerModuleTests()
    : NativeStructTestBase<MibTcp6RowOwnerModule>(192)
{
    [Test]
    public void StaticMembersShouldBeCorrect()
    {
        MibTcp6RowOwnerModule.AddressFamily.ShouldBe(AddressFamily.InterNetworkV6);
        MibTcp6RowOwnerModule.ProtocolType.ShouldBe(ProtocolType.Tcp);
    }
}
