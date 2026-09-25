using Microsoft.Extensions.Logging.Testing;
using ModularPipelines.Logging;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public sealed class FakeModuleLogger : FakeLogger, IModuleLogger
{
    public void Dispose() => throw new NotSupportedException();
}
