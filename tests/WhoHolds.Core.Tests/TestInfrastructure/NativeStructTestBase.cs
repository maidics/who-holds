using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Tests.TestInfrastructure;

internal abstract class NativeStructTestBase<TStruct>(int expectedSize)
    where TStruct : unmanaged, INativeSized<TStruct>
{
    [Test]
    public void ShouldHaveSequentialLayout()
    {
        var layout = typeof(TStruct).StructLayoutAttribute;
        layout.ShouldNotBeNull();
        layout.Value.ShouldBe(LayoutKind.Sequential);
    }

    [Test]
    public void ShouldHaveCorrectSize()
    {
        TStruct.Size.ShouldBe(expectedSize);
    }

    protected static void AssertInlineArrayAttributeAndArrayType(int length, Type arrayType)
    {
        var structType = typeof(TStruct);

        var inlineArray = structType.GetCustomAttribute<InlineArrayAttribute>();
        inlineArray.ShouldNotBeNull();
        inlineArray.Length.ShouldBe(length);

        var fields = structType.GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        fields.Length.ShouldBe(1);

        var field = fields[0];
        field.FieldType.ShouldBe(arrayType);
    }
}
