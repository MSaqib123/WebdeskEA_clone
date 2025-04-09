using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class CategoryController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IProductRepository _ProductRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IModuleRepository _moduleRepo;
        private readonly IImageService _imageService;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;

        public CategoryController(
            IImageService imageService,
            ICategoryRepository CategoryRepository,
            IProductRepository productRepository,
            IPackageRepository packageRepository,
            IPackageTypeRepository packageTypeRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IFinancialYearRepository financialYearRepository,
            ITenantRepository tenantRepository)
        {
            _imageService = imageService;
            _CategoryRepository = CategoryRepository;
            _ProductRepository = productRepository;
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
            _financialYearRepository = financialYearRepository;
            _tenantRepository = tenantRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Category";
            base.OnActionExecuting(context);
        }

        #endregion

        #region Model_Binding
        // GET: Modules/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<CategoryDto> dto = null;
                if (TenantId > 0)
                {
                    dto = await _CategoryRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                CategoryDto dto = new CategoryDto();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //// POST: Modules/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CategoryDto dto)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            dto.TenantId = TenantId;
        //            dto.CompanyId = CompanyId;
        //            var id = await _CategoryRepository.AddAsync(dto);
        //            if (id > 0)
        //            {
        //                return RedirectToAction(nameof(Index));
        //            }
        //            else
        //            {
        //                return View(dto);
        //            }
        //        }
        //        //dto.TenantDtoList = await _tenantRepository.GetAllAsync();
        //        return View(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogError(ex);
        //        return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Process the file if one was uploaded
                    if (dto.ImageForSave != null)
                    {
                        dto.Image = "/uploads/Category/" + _imageService.UploadImage(dto.ImageForSave, "uploads/Category");
                    }

                    // Set additional properties as needed
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;

                    var id = await _CategoryRepository.AddAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View(dto);
                    }
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }



        // GET: Modules/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _CategoryRepository.GetByIdAsync(id);
                if (dto == null)
                    return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var category = await _CategoryRepository.GetByIdAsync(dto.Id);
                    if (category == null)
                        return NotFound();

                    // Preserve existing image if no new image is uploaded
                    if (category != null)
                    {
                        _imageService.DeleteImage(category.Image!, "uploads/Category");
                        dto.Image = "/uploads/Category/" + _imageService.UploadImage(dto.ImageForSave, "uploads/Category");
                    }
                    else
                    {
                        dto.Image = category.Image;
                    }

                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    var id = await _CategoryRepository.UpdateAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                //dto.TenantDtoList = await _tenantRepository.GetAllAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var module = await _CategoryRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                return View(module);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var model = await _CategoryRepository.GetByIdAsync(id);
                if (model == null)
                    return NotFound();
                if (model != null)
                {
                    _imageService.DeleteImage(model.Image, "uploads/Category");
                }

                // Preserve existing image if no new image is uploaded
                var result = await _CategoryRepository.DeleteAsync(id);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete module.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion

        //_______ Error Log ______
        #region LogError
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
