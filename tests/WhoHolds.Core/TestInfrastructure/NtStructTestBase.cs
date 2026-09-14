using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Tests.TestInfrastructure;

internal abstract class NtStructTestBase<TStruct>
    where TStruct : struct, INtStruct
{
    [Test]
    public void ShouldHaveSequentialLayout()
    {
        var layout = typeof(TStruct).StructLayoutAttribute;
        layout.ShouldNotBeNull();
        layout.Value.ShouldBe(LayoutKind.Sequential);
    }

    public abstract void ShouldHaveCorrectSize();

    protected void AssertSize(int size)
    {
        TStruct.GetSize().ShouldBe(size);
    }
}
