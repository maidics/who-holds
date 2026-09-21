using WhoHolds.Core.Common.Models;

namespace WhoHolds.Core.Tests.TestInfrastructure;

internal static class ResultExtensions
{
    extension(Result result)
    {
        public void ShouldBeResultedTo(bool succeeded, params string[] errors)
        {
            result.Succeeded.ShouldBe(succeeded);
            result.Errors.ShouldBe(errors);
        }
    }

    extension<T>(Result<T> result)
    {
        public void ShouldBeResultedTo(bool succeeded, params string[] errors)
        {
            result.Succeeded.ShouldBe(succeeded);
            result.Errors.ShouldBe(errors);
        }
    }
}
