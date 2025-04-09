using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace WebdeskEA.RazerServices
{

    public interface IRazorRenderer
    {
        Task<string> RenderViewToStringAsync(string viewPathOrName, object model);
    }

    public class RazorRenderer : IRazorRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync(string viewPathOrName, object model)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            // Attempt to find the specified view
            var viewResult = _viewEngine.FindView(actionContext, viewPathOrName, isMainPage: false);
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Could not find view '{viewPathOrName}'");
            }

            var view = viewResult.View;

            using var sw = new StringWriter();

            var viewData = new ViewDataDictionary(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(httpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                view,
                viewData,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);

            return sw.ToString();
        }
    }
}
