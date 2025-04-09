using Microsoft.AspNetCore.Mvc;

namespace DataTransformation.Controllers
{
    [Area("Settings")]
    public class ToasterController : Controller
    {
        public IActionResult Index()
        {
            TempData["Success"] = "Inserted Successfully";
            TempData["Success"] = "Updated Successfully";
            TempData["Success"] = "Deleted Successfully";
            TempData["Error"] = "Error during Request";
            TempData["Update"] = "Update message ";
            return View();
        }
    }
}
