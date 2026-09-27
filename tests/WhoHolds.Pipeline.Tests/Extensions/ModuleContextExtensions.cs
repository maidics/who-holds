using ModularPipelines.Context;
using ModularPipelines.Context.Domains;
using ModularPipelines.DotNet.Services;

namespace WhoHolds.Pipeline.Tests.Extensions;

public static class ModuleContextExtensions
{
    extension(IModuleContext)
    {
        public static IModuleContext CreateWithDotNetMock(IDotNetMock dotnet)
        {
            var services = IServicesContext.Mock();
            services.Get<IDotNet>().Returns(dotnet.Object);

            var context = IModuleContext.Mock();
            context.Services.Returns(services.Object);

            return context.Object;
        }
    }
}
