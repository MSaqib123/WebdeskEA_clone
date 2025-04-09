using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.Utility;

namespace WebdeskEA.Domain.ServiceMiddlwares
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public CustomExceptionHandlingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.Redirect($"/Settings/Error/Error?statusCode=403&errorMessage={Uri.EscapeDataString(ex.Message)}");
                await LogError(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.Redirect($"/Settings/Error/Error?statusCode=500&errorMessage={Uri.EscapeDataString(ex.Message)}");
                await LogError(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorLogRepository = scope.ServiceProvider.GetRequiredService<IErrorLogRepository>();

            var routeData = context.GetRouteData();
            string areaName = routeData?.Values["area"]?.ToString() ?? "Area Not Found";
            string controllerName = routeData?.Values["controller"]?.ToString() ?? "Controller Not Found";
            string actionName = routeData?.Values["action"]?.ToString() ?? "Action Not Found";

            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);

            await errorLogRepository.AddErrorLogAsync(
                area: areaName,
                controller: controllerName,
                actionName: actionName,
                formName: $"{actionName} Form",
                errorShortDescription: ex.Message,
                errorLongDescription: ErrorUtility.GetFullExceptionMessage(ex),
                statusCode: statusCode.ToString(),
                username: context.User.Identity?.Name ?? "Unknown User"
            );
        }
    }

}
