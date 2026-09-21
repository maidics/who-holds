using WhoHolds.Core.Interop.Structs;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Interop.Structs;

[InheritsTests]
internal sealed class FileTimeTests() : NativeStructTestBase<FileTime>(8)
{
    [Test]
    public void ToDateTimeUtcShouldConvertCorrectly()
    {
        var expected = new DateTime(2026, 9, 18, 12, 34, 56, DateTimeKind.Utc);
        var ticks = expected.ToFileTimeUtc();

        var fileTime = new FileTime((uint)ticks, (uint)(ticks >> 32));
        fileTime.ToDateTimeUtc().ShouldBe(expected);
    }

    [Test]
    public void ToDateTimeUtcShouldNotSignExtendLowPart()
    {
        var fileTime = new FileTime(0xFFFFFFFF, 1);

        fileTime.ToDateTimeUtc().ShouldBe(DateTime.FromFileTimeUtc(0x1_FFFFFFFF));
    }
}
