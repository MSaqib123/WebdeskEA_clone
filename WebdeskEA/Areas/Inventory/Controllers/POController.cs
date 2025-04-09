using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class POController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IPIRepository _PIRepository;
        private readonly IPOVATBreakdownRepository _POVATBreakdownRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPODtlTaxRepository _PODtlTaxRepository;
        private readonly IPODtlRepository _PODtlRepository;
        private readonly IPORepository _PORepository;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ICOARepository _COARepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        private readonly IImageService _imageService;
        private readonly ICompanyRepository _companyRepository;
        public POController(
            IImageService imageService,
            ICompanyRepository companyRepository,
            ISupplierRepository SupplierRepository,
            IProductCOARepository ProductCOARepository,
            ICOARepository COARepository,
            ITaxMasterRepository taxMasterRepository,
            IBrandRepository brandRepository,
            IProductRepository productRepository,
            IPackageRepository packageRepository,
            IPackageTypeRepository packageTypeRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IFinancialYearRepository financialYearRepository,
            ITenantRepository tenantRepository,
            IPODtlRepository sODtlRepository,
            IPORepository sORepository,
            ITenantPrefixService tenantPrefixService,
            IPODtlTaxRepository PODtlTaxRepository,
            IPOVATBreakdownRepository POVATBreakdownRepository,
            IPIRepository PIRepository)
        {
            _companyRepository = companyRepository;
            _imageService = imageService;
            _supplierRepository = SupplierRepository;
            _productCOARepository = ProductCOARepository;
            _COARepository = COARepository;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
            _financialYearRepository = financialYearRepository;
            _tenantRepository = tenantRepository;
            _taxMasterRepository = taxMasterRepository;
            _PODtlRepository = sODtlRepository;
            _PORepository = sORepository;
            _tenantPrefixService = tenantPrefixService;
            _PIRepository = PIRepository;
            _PODtlTaxRepository = PODtlTaxRepository;
            _POVATBreakdownRepository = POVATBreakdownRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Purchase Order";
            base.OnActionExecuting(context);
        }
        #endregion
        
        //_______ Partial Binding ______
        #region Model_Binding
        // GET: Modules/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<PODto> dto;
                if (TenantId > 0)
                {
                    dto = await _PORepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _PORepository.GetAllAsync();
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
                PODto dto = new PODto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.POCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.PO, true);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
        public async Task<IActionResult> Create(PODto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PORepository.AddTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                var dto = await _PORepository.GetByIdAsync(id) ?? new PODto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(id);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlTaxDtos = await _PODtlTaxRepository.GetAllByPOIdAsync(id);
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
        public async Task<IActionResult> Edit(PODto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PORepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(id) ?? new PODto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(id);
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

        // GET: Modules/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(id);
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

        // POST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(id);
                if (dto == null)
                    return NotFound();

                var (isSuccess, message) = await _PORepository.ForceDeletePOAsync(id);

                if (isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", message);
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }



        // GET: Modules/Detail/{id}
        [HttpGet]
        public async Task<IActionResult> DetailsPrint(int id)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(id) ?? new PODto();
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlTaxDtos = await _PODtlTaxRepository.GetAllByPOIdAsync(id);
                dto.CompanyDto = await _companyRepository.GetByIdAsync(CompanyId);
                dto.CompanyDto.Logo = _imageService.GetImagePath(dto.CompanyDto!.Logo, "~/uploads/Company/Logo/") ?? "~/Template/img/160x160/img1.jpg";
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

        #endregion

        //_______ Partial Binding ______
        #region Partial_Binding

        //====================================================
        //======== For Other Controller | Views ==============
        //====================================================
        #region For Other Controller | Views 
        /// <summary>
        /// this is being used in PI  on  PO Drpdown to fetch the SO list in SI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetPOById(int id)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(id) ?? new PODto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(id);
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                // Return the JSON response
                return Json(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }

        }

        #endregion

        //====================================================
        //======== For Current Controller | Views  ===========
        //====================================================
        #region For Current Controller | Views 
        //SO -> Create Copy of SI

        /// <summary>
        /// this is being used to  Check PI exsit or not 
        /// </summary>
        /// <param name="soId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckPIExists(int poId)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(poId) ?? new PODto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                return Json(new { success = true, entityExists = dto.IsPIExist, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpGet]
        public async Task<IActionResult> GotoEditPI(int poId)
        {
            try
            {
                var dto = await _PIRepository.GetByPOIdAsync(poId) ?? new PIDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                if (dto.POId > 0)
                {
                    var editUrl = Url.Action("Edit", "PI", new { id = dto.Id, area = "Inventory" });
                    return Json(new { success = true, entityUrl = editUrl, entityExists = true, data = dto });
                }
                else
                {
                    return Json(new { success = false, entityExists = false, data = dto });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }


        [HttpGet]
        public async Task<IActionResult> GotoDetailPI(int poId)
        {
            try
            {
                var dto = await _PIRepository.GetByPOIdAsync(poId) ?? new PIDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                if (dto.POId > 0)
                {
                    var editUrl = Url.Action("Details", "PI", new { id = dto.Id, area = "Inventory" });
                    return Json(new { success = true, entityUrl = editUrl, entityExists = true, data = dto });
                }
                else
                {
                    return Json(new { success = false, entityExists = false, data = dto });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }


        public class GeneratePIRequest
        {
            public int PoId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePI([FromBody] GeneratePIRequest request)
        {
            try
            {

                var siResult = await _PIRepository.GetByPOIdAsync(request.PoId) ?? new PIDto();
                if (siResult.Id > 0)
                    return Json(new { success = false, message = "Invoice Alredy Generated." });

                var dto = await _PORepository.GetByIdAsync(request.PoId) ?? new PODto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.FinancialYearId = FinancialYearId;
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(dto.Id);
                dto.PODtlTaxDtos = await _PODtlTaxRepository.GetAllByPOIdAsync(dto.Id);
                dto.POVATBreakdownDtos = await _POVATBreakdownRepository.GetAllByPOIdAsync(dto.Id);
                dto.POCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.PI, true);
                var result = await _PORepository.GeneratePIByPO(dto);
                if (!result)
                    return Json(new { success = false, message = "Invoice not found." });

                return Json(new { success = true, entityExists = dto.IsPIExist, message = "Invoice Generated Successfully" });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePI([FromBody] GeneratePIRequest request)
        {
            try
            {
                var dto = await _PORepository.GetByIdAsync(request.PoId) ?? new PODto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.PODtlDtos = await _PODtlRepository.GetAllByPOIdAsync(dto.Id);

                var result = await _PORepository.DeletePIByPO(request.PoId);
                if (!result)
                    return Json(new { success = false, message = "SalesOrder not found." }); // Return failure response for AJAX

                // Return the JSON response
                return Json(new { success = true, entityExists = dto.IsPIExist, message = "Invoice Generated Successfully" });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
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
