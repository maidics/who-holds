using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Exceptions;

namespace WhoHolds.Core.Tests.Interop;

internal sealed class SystemQueryTests
{
    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public void ShouldThrowArgumentOutOfRangeExceptionIfInitialSizeIsLessThanOrEqualZero(
        int initialSize
    )
    {
        var fake = new FakeQuery(0);

        Should.Throw<ArgumentOutOfRangeException>(() =>
            SystemQuery.QueryWithGrowingBuffer(
                fake.Invoke,
                SystemClass.SystemBasicInformation,
                out _,
                initialSize
            )
        );
    }

    [Test]
    public void ShouldGrowUntilBufferIsLargeEnough()
    {
        var fake = new FakeQuery(required: 3_000_000);

        using var buffer = SystemQuery.QueryWithGrowingBuffer(
            fake.Invoke,
            SystemClass.SystemHandleInformationEx,
            out _,
            initialSize: 1024
        );

        fake.Sizes.ShouldBe(new[] { 1024, 3_750_000 });
        buffer.Size.ShouldBe(3_750_000);
    }

    [Test]
    public void ShouldGiveUpAfterEightAttempts()
    {
        var fake = new FakeQuery(
            required: int.MaxValue,
            returnLength: 0,
            returnStatus: NtStatus.InfoLengthMismatch
        );

        var ex = Should.Throw<InvalidOperationException>(() =>
            SystemQuery.QueryWithGrowingBuffer(
                fake.Invoke,
                SystemClass.SystemBasicInformation,
                out _,
                initialSize: 1 << 5
            )
        );

        ex.Message.ShouldStartWith("Buffer size never converged");

        fake.Sizes.Count.ShouldBe(8);
    }

    [Test]
    public void ShouldThrowOnUnexpectedStatus()
    {
        Should.Throw<NtException>(() =>
            SystemQuery.QueryWithGrowingBuffer(
                (SystemClass c, IntPtr b, int l, out uint r) =>
                {
                    r = 0;
                    return NtStatus.InvalidHandle;
                },
                SystemClass.SystemBasicInformation,
                out _,
                initialSize: 1024
            )
        );
    }

    private sealed class FakeQuery
    {
        public List<int> Sizes { get; } = new();
        private readonly uint _required;
        private readonly uint? _returnLength;
        private readonly NtStatus? _status;

        public FakeQuery(uint required, uint? returnLength = null, NtStatus? returnStatus = null)
        {
            _required = required;
            _returnLength = returnLength;
            _status = returnStatus;
        }

        public NtStatus Invoke(
            SystemClass systemClass,
            IntPtr buffer,
            int length,
            out uint returnLength
        )
        {
            Sizes.Add(length);
            returnLength = _returnLength ?? _required;

            if (_status is not null)
                return _status.Value;

            return length >= _required ? NtStatus.Success : NtStatus.InfoLengthMismatch;
        }
    }
}
