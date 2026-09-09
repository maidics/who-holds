using Shouldly;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Tests.Utility;

public sealed class ByteFormatTests
{
    [Test]
    [Arguments(1, "1 B")]
    [Arguments(100, "100 B")]
    [Arguments(1 << 10, "1 KB")]
    [Arguments(1 << 20, "1 MB")]
    [Arguments(1 << 30, "1 GB")]
    [Arguments(1L << 40, "1024 GB")]
    public void ShouldReturnCorrectFormat(long bytes, string expected)
    {
        ByteFormat.Humanize(bytes).ShouldBe(expected);
    }
}
