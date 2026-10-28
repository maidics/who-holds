using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

namespace WhoHolds.Core.Tests.Architecture;

public sealed class PortRowOwnerModuleImplementationTests
{
    [Test]
    public void InterfaceShouldBeImplementedByCorrectStructs()
    {
        var assembly = typeof(Libraries).Assembly;

        var portRowOwnerModuleTypes = assembly
            .GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsInterface: false }
                && t.GetInterfaces().Any(i => i == typeof(IPortRowOwnerModule))
            );

        portRowOwnerModuleTypes.Count().ShouldBe(4);

        typeof(MibTcpRowOwnerModule).IsAssignableTo(typeof(IPortRowOwnerModule)).ShouldBeTrue();
        typeof(MibTcp6RowOwnerModule).IsAssignableTo(typeof(IPortRowOwnerModule)).ShouldBeTrue();
        typeof(MibUdpRowOwnerModule).IsAssignableTo(typeof(IPortRowOwnerModule)).ShouldBeTrue();
        typeof(MibUdp6RowOwnerModule).IsAssignableTo(typeof(IPortRowOwnerModule)).ShouldBeTrue(); // nothing else can implement this interface
    }
}
