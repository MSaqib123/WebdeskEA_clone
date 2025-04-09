using WebdeskEA.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebdeskEA.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Error(int statusCode, string errorMessage)
        {
            _logger.LogError($"Error {statusCode}: {errorMessage}");

            var errorViewModel = new ErrorViewModel
            {
                StatusCode = statusCode,
                ErrorMessage = GetErrorMessage(statusCode, errorMessage)
            };
            return View(errorViewModel);
        }
        public IActionResult UnAuthorizedAccess()
        {
            _logger.LogError($"Error {401}: UnAuthorized");

            var errorViewModel = new ErrorViewModel
            {
                StatusCode = 401,
                ErrorMessage = GetErrorMessage(403,"")
            };
            return View(errorViewModel);
        }

        private string GetErrorMessage(int statusCode, string errorMessage)
        {
            return statusCode switch
            {
                403 => "You are forbidden to access this page.",
                401 => "You are not authorized to access this page.",
                404 => "The page you're looking for was not found.",
                500 => "An unexpected error occurred.",
                _ => errorMessage
            };
        }
    }
}
