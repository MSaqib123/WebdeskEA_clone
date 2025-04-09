using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;

namespace CRM.Areas.CompanySites.Controllers
{
    [Area("CompanySites")]
    [Authorize(Roles = "Tenant Admin,SuperAdmin,Admin,AuthUser")]
    public class CompanyBusinessCategoryController : Controller
    {
        //=================== Constructor =====================
        #region Constructor
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly ICompanyBusinessCategoryRepository _companyBusinessCategoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CompanyBusinessCategoryController(ICompanyUserRepository companyUserRepository,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository,
            ICompanyBusinessCategoryRepository companyBusinessCategoryRepository
            )
        {
            _companyUserRepository = companyUserRepository;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
            _companyBusinessCategoryRepository = companyBusinessCategoryRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Company BusinessCategory";
            base.OnActionExecuting(context);
        }

        #endregion

        //=================== Model_Binding =====================
        #region Model_Binding
        //=================== Create =====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var companies = await _CompanyRepository.GetAllAsync();
                return View(companies);
            }
            catch (Exception ex)
            {
                // Log the error
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //=================== Create =====================
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            try
            {
                CompanyBusinessCategoryDto dto = new CompanyBusinessCategoryDto();
                CompanyDto company = await _CompanyRepository.GetByIdAsync(id);
                dto.CompanyId = company.Id;
                dto.CompanyName = company.Name;
                dto.BusinessCategories = await _companyBusinessCategoryRepository.GetAllCompanyBusinessCategoryByCompanyIdAsync(id);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        //==================== Delete ======================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var companyUsers = await _companyUserRepository.GetAllCompanyUserByCompanyIdAsync(id);
                if (companyUsers == null || !companyUsers.Any())
                {
                    return NotFound();
                }

                var companyDetail = companyUsers.FirstOrDefault();

                ViewBag.CompanyId = companyDetail?.CompanyId;
                ViewBag.CompanyName = companyDetail?.CompanyName;

                return View(companyUsers);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _companyUserRepository.DeleteCompanyUserAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        //=================== Detail =====================
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                CompanyBusinessCategoryDto dto = new CompanyBusinessCategoryDto();
                CompanyDto company = await _CompanyRepository.GetByIdAsync(id);
                dto.CompanyId = company.Id;
                dto.CompanyName = company.Name;
                dto.BusinessCategories = await _companyBusinessCategoryRepository.GetAllCompanyBusinessCategoryByCompanyIdAsync(id);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        #endregion
        
        //=================== Partial Binding =====================
        #region Partial_Binding
        //=== Create ===
        [HttpPost]
        public async Task<IActionResult> CreateBusinessCategory([FromBody] CompanyBusinessCategoryDto Dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int returnId = await _companyBusinessCategoryRepository.AddCompanyBusinessCategoryAsync(Dto);
                    var InsertedRecord = await _companyBusinessCategoryRepository.GetCompanyBusinessCategoryByIdAsync(returnId);
                    return Json(new { success = true, message = "Category created successfully." , obj = InsertedRecord });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid data.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompanyBusinessCategory(int id, [FromBody] CompanyBusinessCategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = await _companyBusinessCategoryRepository.GetCompanyBusinessCategoryByIdAsync(id);
                    if (existingCategory == null)
                    {
                        return Json(new { success = false, message = "BusinessCategory not found." });
                    }

                    await _companyBusinessCategoryRepository.UpdateCompanyBusinessCategoryAsync(dto);
                    return Json(new { success = true, message = "Category updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid data.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompanyBusinessCategory(int id)
        {
            try
            {
                // Find and delete the category
                var existingCategory = await _companyBusinessCategoryRepository.GetCompanyBusinessCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    return Json(new { success = false, message = "BusinessCategory not found." });
                }
                await _companyBusinessCategoryRepository.DeleteCompanyBusinessCategoryAsync(existingCategory.Id);
                return Json(new { success = true, message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        //=================== Error Handling =====================
        #region ErrorHandling
        private async Task LogError(Exception ex)
        {
            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);

            string areaName = ControllerContext.RouteData.Values["area"]?.ToString() ?? "Area Not Found";
            string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString() ?? "Controler Not Found";
            string actionName = ControllerContext.RouteData.Values["action"]?.ToString() ?? "ActionName not Found";

            await _errorLogRepository.AddErrorLogAsync(
                area: areaName,
                controller: controllerName,
                actionName: actionName,
                formName: $"{actionName} Form",
                errorShortDescription: ex.Message,
                errorLongDescription: ErrorUtility.GetFullExceptionMessage(ex),
                statusCode: statusCode.ToString(),
                username: User.Identity?.Name ?? "GuestUser"
            );
        }
        #endregion

    }
}
