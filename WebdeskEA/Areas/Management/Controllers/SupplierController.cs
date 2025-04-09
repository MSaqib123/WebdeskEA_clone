using AutoMapper;
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

namespace WebdeskEA.Areas.Management.Controllers
{
    [Area("Management")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class SupplierController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        private readonly ICompanyRepository _companyRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICOARepository _COARepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        public SupplierController(
            IPackageRepository packageRepository,
            IPackageTypeRepository packageTypeRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IFinancialYearRepository financialYearRepository,
            ITenantRepository tenantRepository,
            ICustomerRepository customerRepository,
            ISupplierRepository supplierRepository,
            ICompanyRepository companyRepository,
            ICOARepository COARepository,
            ITenantPrefixService tenantPrefixService
            )
        {
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
            _financialYearRepository = financialYearRepository;
            _tenantRepository = tenantRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _companyRepository = companyRepository;
            _COARepository = COARepository;
            _tenantPrefixService = tenantPrefixService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Supplier";
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
                if (TenantId > 0)
                {
                    var dto = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    return View(dto);
                }
                else
                {
                    var dto = "";//await _supplierRepository.GetAllAsync();
                    return View(dto);
                }
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
                SupplierDto dto = new SupplierDto();
                dto.CompanyDtos = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (TenantId > 0)
                {
                    dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, dto.Code, PrefixType.Supplier, true);
                    dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else
                {
                   // dto.COADtos = await _COARepository.GetAllAsync();
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.CompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, dto.Code, PrefixType.Supplier, false);
                    var id = await _supplierRepository.AddAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View(dto);
                    }
                }
                dto.CompanyDtos = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (TenantId > 0)
                {
                    dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else
                {
                    dto.COADtos = await _COARepository.GetAllAsync();
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
                var dto = await _supplierRepository.GetByIdAsync(id);
                dto.CompanyDtos = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (TenantId > 0)
                {
                    dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else
                {
                    dto.COADtos = await _COARepository.GetAllAsync();
                }
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
        public async Task<IActionResult> Edit(SupplierDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.CompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    var id = await _supplierRepository.UpdateAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                dto.CompanyDtos = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (TenantId > 0)
                {
                    dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else
                {
                    dto.COADtos = await _COARepository.GetAllAsync();
                }
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
                var module = await _supplierRepository.GetByIdAsync(id);
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
                var module = await _supplierRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                var result = await _supplierRepository.DeleteAsync(id);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete module.");
                    return View(module);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion

        //_______ Helper Method ______
        #region Helper_Method
        private async Task<SupplierDto> BuildObjectAsync()
        {
            var dto = new SupplierDto();
            dto.CompanyDtos = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            if (TenantId > 0)
            {
                dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, dto.Code, PrefixType.Supplier, true);
                dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            }
            else
            {
                // dto.COADtos = await _COARepository.GetAllAsync();
            }
            return dto;
        }
        #endregion

        //_______ Partial Binding _________
        #region Partial_Binding

        //============== Openig Modal ==============
        #region Supplier_Opening_Modal
        [HttpGet]
        public async Task<IActionResult> CreatePartial() //Purchase ?? Sale
        {
            try
            {
                SupplierDto dto = await BuildObjectAsync();
                return PartialView("_CreateSupplierPartial", dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to load product form: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePartialPost(SupplierDto model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateSupplierPartial", model);
            }

            try
            {
                // Save the product
                model.TenantId = TenantId;
                model.CompanyId = CompanyId;
                var newId = await _supplierRepository.AddAsync(model);

                // Return JSON so the client can close the modal and refresh product dropdown
                return Json(new
                {
                    success = true,
                    newId = newId,
                    newName = model.Name,
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
                return PartialView("_CreateSupplierPartial", model);
            }
        }
        #endregion


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
