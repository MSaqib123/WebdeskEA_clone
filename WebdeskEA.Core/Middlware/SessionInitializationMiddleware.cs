using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WebdeskEA.Core.Middlware;

namespace WebdeskEA.Core.Middleware
{
    public class SessionInitializationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionInitializationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // Resolve the scoped UserSessionService within the request scope
                var userSessionService = scope.ServiceProvider.GetRequiredService<UserSessionService>();
                await userSessionService.InitializeUserSessionIfNeededAsync();
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
