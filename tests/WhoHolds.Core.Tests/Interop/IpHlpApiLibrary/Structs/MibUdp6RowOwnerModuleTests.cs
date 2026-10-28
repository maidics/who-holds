using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class MibUdp6RowOwnerModuleTests()
    : NativeStructTestBase<MibUdp6RowOwnerModule>(176)
{
    [Test]
    public void StaticMembersShouldBeCorrect()
    {
        MibUdp6RowOwnerModule.AddressFamily.ShouldBe(AddressFamily.InterNetworkV6);
        MibUdp6RowOwnerModule.ProtocolType.ShouldBe(ProtocolType.Udp);
    }
}
