using ModularPipelines.Enums;
using ModularPipelines.Models;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class ModuleResultExtensions
{
    extension(ModuleResult)
    {
        public static ModuleResult<T?> CreateSuccess<T>(T? value)
        {
            return new ModuleResult<T?>.Success(value)
            {
                ModuleName = typeof(T).Name,
                ModuleDuration = TimeSpan.Zero,
                ModuleStart = DateTimeOffset.UtcNow,
                ModuleEnd = DateTimeOffset.UtcNow,
                ModuleStatus = Status.Successful,
            };
        }
    }
}
