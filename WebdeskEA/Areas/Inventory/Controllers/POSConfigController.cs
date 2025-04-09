using AutoMapper;
using Azure.Core;
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
    public class POSConfigController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IPOSConfigRepository _POSConfigRepository;
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

        public POSConfigController(
            IImageService imageService,
            ICustomerRepository CustomerRepository,
            IPOSConfigRepository POSConfigRepository,
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
            _CustomerRepository = CustomerRepository;
            _imageService = imageService;
            _POSConfigRepository = POSConfigRepository;
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
            ViewBag.NameOfForm = "POSConfig";
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
                IEnumerable<POSConfigDto> dto = null;
                if (TenantId > 0)
                {
                    dto = await _POSConfigRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                POSConfigDto dto = new POSConfigDto();
                dto.CustomerDtos = await _CustomerRepository.GetAllByTenantCompanyIdAsync(TenantId,CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(POSConfigDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set additional properties as needed
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;

                    var id = await _POSConfigRepository.AddAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        dto.CustomerDtos = await _CustomerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                var dto = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(id,TenantId,CompanyId);
                dto.CustomerDtos = await _CustomerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
        public async Task<IActionResult> Edit(POSConfigDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var POSConfig = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(dto.Id,TenantId,CompanyId);
                    if (POSConfig == null)
                        return NotFound();

                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    var id = await _POSConfigRepository.UpdateAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        dto.CustomerDtos = await _CustomerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        ModelState.AddModelError("", "Failed to update module.");
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
        public async Task<IActionResult> ActiveDeactive(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }

                var POSConfig = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(id.Value,TenantId,CompanyId);
                if (POSConfig == null)
                {
                    return NotFound();
                }

                POSConfig.TenantId = TenantId;
                POSConfig.CompanyId = CompanyId;

                // Toggle the IsCurrentActive status
                POSConfig.IsCurrentActive = !POSConfig.IsCurrentActive;

                var updateResult = await _POSConfigRepository.UpdateAsync(POSConfig);
                if (updateResult > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update module.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ActiveDeactive([FromBody] ActiveDeactiveRequest request)
        {
            try
            {
                int id = request.Id;
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid ID received." });
                }
                var POSConfig = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(id,TenantId,CompanyId);
                if (POSConfig == null)
                {
                    return Json(new { success = false, message = "Record not found." });
                }

                POSConfig.IsCurrentActive = !POSConfig.IsCurrentActive;
                var updateResult = await _POSConfigRepository.UpdateAsync(POSConfig);

                if (updateResult > 0)
                {
                    return Json(new
                    {
                        success = true,
                        id = id,
                        isActive = POSConfig.IsCurrentActive,
                        tax = POSConfig.DefaultTax
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update status." });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class ActiveDeactiveRequest
        {
            public int Id { get; set; }
        }



        // GET: Modules/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var module = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(id,TenantId,CompanyId);
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
                var model = await _POSConfigRepository.GetByIdTenantIdCompanyIdAsync(id,TenantId,CompanyId);
                if (model == null)
                    return NotFound();
               
                // Preserve existing image if no new image is uploaded
                var result = await _POSConfigRepository.DeleteAsync(id);
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
