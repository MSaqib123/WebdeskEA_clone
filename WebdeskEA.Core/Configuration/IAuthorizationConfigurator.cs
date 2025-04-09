using Microsoft.Extensions.DependencyInjection;

namespace WebdeskEA.Core.Configuration
{
    public interface IAuthorizationConfigurator
    {
        Task ConfigurePolicies(IServiceCollection services);
    }
}