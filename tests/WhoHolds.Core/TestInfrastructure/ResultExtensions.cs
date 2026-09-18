using WhoHolds.Core.Common.Models;

namespace WhoHolds.Core.Tests.TestInfrastructure;

//TODO: refactor tests with this
internal static class ResultExtensions
{
    extension(Result result)
    {
        public void ShouldBeResultedTo(ResultType type, params string[] errors)
        {
            result.Type.ShouldBe(type);
            result.Errors.ShouldBe(errors);
        }
    }

    extension<T>(Result<T> result)
    {
        public void ShouldBeResultedTo(ResultType type, params string[] errors)
        {
            result.Type.ShouldBe(type);
            result.Errors.ShouldBe(errors);
        }
    }
}
