using ModularPipelines.Context;
using ModularPipelines.Context.Domains;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Logging;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class ModuleContextExtensions
{
    extension(IModuleContext)
    {
        public static IModuleContext Create(IDotNetMock dotnet, IModuleLogger? logger = null)
        {
            var services = IServicesContext.Mock();
            services.Get<IDotNet>().Returns(dotnet.Object);

            var context = IModuleContext.Mock();
            context.Services.Returns(services.Object);

            if (logger is not null)
            {
                context.Logger.Returns(logger);
            }

            return context.Object;
        }
    }
}
