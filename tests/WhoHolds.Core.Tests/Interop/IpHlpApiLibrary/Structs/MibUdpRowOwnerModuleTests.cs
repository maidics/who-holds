using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class MibUdpRowOwnerModuleTests() : NativeStructTestBase<MibUdpRowOwnerModule>(160)
{
    [Test]
    public void StaticMembersShouldBeCorrect()
    {
        MibUdpRowOwnerModule.AddressFamily.ShouldBe(AddressFamily.InterNetwork);
        MibUdpRowOwnerModule.ProtocolType.ShouldBe(ProtocolType.Udp);
    }
}
