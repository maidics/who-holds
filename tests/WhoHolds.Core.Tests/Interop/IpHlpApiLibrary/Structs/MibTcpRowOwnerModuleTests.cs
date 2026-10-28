using System.Net.Sockets;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.IpHlpApiLibrary.Structs;

[InheritsTests]
internal sealed class MibTcpRowOwnerModuleTests() : NativeStructTestBase<MibTcpRowOwnerModule>(160)
{
    [Test]
    public void StaticMembersShouldBeCorrect()
    {
        MibTcpRowOwnerModule.AddressFamily.ShouldBe(AddressFamily.InterNetwork);
        MibTcpRowOwnerModule.ProtocolType.ShouldBe(ProtocolType.Tcp);
    }
}
