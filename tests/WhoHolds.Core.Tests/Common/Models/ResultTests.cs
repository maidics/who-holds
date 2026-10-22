using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Tests.TestInfrastructure;

namespace WhoHolds.Core.Tests.Common.Models;

public sealed class ResultTests
{
    [Test]
    public void ConstructorShouldNotThrowIfSucceededIsTrueAndErrorsIsNotEmpty()
    {
        var result = new Result(true, "test");
        result.ShouldBeResultedTo(true, "test");
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [Arguments(false, "test")]
    public void ConstructorShouldCreateResult(bool succeeded, params string[] errors)
    {
        var result = new Result(succeeded, errors);
        result.ShouldBeResultedTo(succeeded, errors);
    }

    [Test]
    public void SuccessShouldCreateSucceededResult()
    {
        var result = Result.Success();
        result.ShouldBeResultedTo(true);
    }

    [Test]
    public void GenericSuccessMethodShouldCreateSucceededResult()
    {
        var result = Result.Success(10);
        result.ShouldBeResultedTo(true);
        result.Value.ShouldBe(10);
    }

    [Test]
    public void FailureMethodShouldCreateResultFailure()
    {
        var failure = Result.Failure("test");
        failure.Errors.ShouldBe(["test"]);
    }

    [Test]
    public void ImplicitOperatorShouldConvertResultFailureToResult()
    {
        var failure = Result.Failure("test");
        Result result = failure;
        result.ShouldBeResultedTo(false, "test");
    }

    [Test]
    public void GenericResultConstructorShouldThrowIfSucceededIsTrueAndErrorIsNotEmpty()
    {
        var result = new Result<int>(true, 10, "test");
        result.Value.ShouldBe(10);
        result.ShouldBeResultedTo(true, "test");
    }

    [Test]
    public void ValueGetterShouldThrowIfSucceededIsFalse()
    {
        var result = new Result<int>(false, 10, "test");

        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Test]
    public void GenericResultImplicitOperatorShouldConvertResultFailureToGenericResult()
    {
        var failure = Result.Failure("test");
        Result<int> result = failure;
        result.ShouldBeResultedTo(false, "test");
    }
}
