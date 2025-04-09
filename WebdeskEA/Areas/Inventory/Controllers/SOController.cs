using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
    public class SOController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);


        private readonly ISODtlTaxRepository _SODtlTaxRepository;
        private readonly ISOVATBreakdownRepository _SOVATBreakdownRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISODtlRepository _SODtlRepository;
        private readonly ISIRepository _SIRepository;
        private readonly ISORepository _SORepository;
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
        private readonly ICompanyRepository _companyRepository;
        private readonly IImageService _imageService;
        public SOController(
            ICompanyRepository companyRepository,
            IImageService imageService,
            ICustomerRepository customerRepository,
            IProductCOARepository ProductCOARepository,
            ISOVATBreakdownRepository SOVATBreakdownRepository,
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
            ISODtlRepository sODtlRepository,
            ISORepository sORepository,
            ITenantPrefixService tenantPrefixService,
            ISODtlTaxRepository SODtlTaxRepository,
            ISIRepository SIRepository)
        {
            _customerRepository = customerRepository;
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
            _SODtlRepository = sODtlRepository;
            _SORepository = sORepository;
            _SIRepository = SIRepository;
            _tenantPrefixService = tenantPrefixService;
            _SODtlTaxRepository = SODtlTaxRepository;
            _imageService = imageService;
            _companyRepository = companyRepository;
            _SOVATBreakdownRepository = SOVATBreakdownRepository;

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Sale Order";
            base.OnActionExecuting(context);
        }
        #endregion

        //_______ Model Binding ______
        #region Model_Binding
        // GET: Modules/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<SODto> dto;
                if (TenantId > 0)
                {
                    dto = await _SORepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = await _SORepository.GetAllAsync();
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
                SODto dto = new SODto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SOCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.SO, true);
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
        public async Task<IActionResult> Create(SODto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _SORepository.AddTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SOCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SO, true);
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
                var dto = await _SORepository.GetByIdAsync(id) ?? new SODto();
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(id);
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
        public async Task<IActionResult> Edit(SODto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _SORepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update SO.");
                    }
                }
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(dto.Id);
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
                var dto = await _SORepository.GetByIdAsync(id);
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(id);
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
                var dto = await _SORepository.GetByIdAsync(id);
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                    return NotFound();

                var (isSuccess, message) = await _SORepository.ForceDeleteSOAsync(id);
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


        // GET: Modules/Detail/{id}ss
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var dto = await _SORepository.GetByIdAsync(id);
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(id);
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


        // GET: Modules/Detail/{id}ss
        [HttpGet]
        public async Task<IActionResult> DetailsPrint(int id)
        {
            try
            {
                var dto = await _SORepository.GetByIdAsync(id) ?? new SODto();
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(id);
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
        /// this is being used in SI  on  SO Drpdown to fetch the SO list in SI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetSOById(int id)
        {
            try
            {
                var dto = await _SORepository.GetByIdAsync(id) ?? new SODto();
                // Fetch related data
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(id);

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
        /// this is being used to  Check SI exsit or not 
        /// </summary>
        /// <param name="soId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckSIExists(int soId)
        {
            try
            {
                var dto = await _SORepository.GetByIdAsync(soId) ?? new SODto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                return Json(new { success = true, entityExists = dto.IsSIExist, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpGet]
        public async Task<IActionResult> GotoEditSI(int soId)
        {
            try
            {
                var dto = await _SIRepository.GetBySOIdAsync(soId) ?? new SIDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                if (dto.SOId > 0)
                {
                    var editUrl = Url.Action("Edit", "SI", new { id = dto.Id, area = "Inventory" });
                    Console.WriteLine(editUrl);
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
        public async Task<IActionResult> GotoDetailSI(int soId)
        {
            try
            {
                var dto = await _SIRepository.GetBySOIdAsync(soId) ?? new SIDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." });

                if (dto.SOId > 0)
                {
                    var editUrl = Url.Action("Details", "SI", new { id = dto.Id, area = "Inventory" });
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
                return Json(new { success = false, message = ex.Message });
            }
        }


        public class GenerateSIRequest
        {
            public int SoId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSI([FromBody] GenerateSIRequest request)
        {
            try
            {

                var siResult = await _SIRepository.GetBySOIdAsync(request.SoId) ?? new SIDto();
                if (siResult.Id > 0)
                   return Json(new { success = false, message = "Invoice Alredy Generated." });

                var dto = await _SORepository.GetByIdAsync(request.SoId) ?? new SODto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.FinancialYearId = FinancialYearId;
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);
                dto.SODtlTaxDtos = await _SODtlTaxRepository.GetAllBySOIdAsync(dto.Id);
                dto.SOVATBreakdownDtos = await _SOVATBreakdownRepository.GetAllBySOIdAsync(dto.Id);
                dto.SOCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.SI, true);
                var result = await _SORepository.GenerateSIBySO(dto);
                if (!result)
                    return Json(new { success = false, message = "Invoice not found." });

                return Json(new { success = true, entityExists = dto.IsSIExist , message = "Invoice Generated Successfully" });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSI([FromBody] GenerateSIRequest request)
        {
            try
            {
                var dto = await _SORepository.GetByIdAsync(request.SoId) ?? new SODto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.SODtlDtos = await _SODtlRepository.GetAllBySOIdAsync(dto.Id);

                var result = await _SORepository.DeleteSIBySO(request.SoId);
                if (!result)
                    return Json(new { success = false, message = "SalesOrder not found." }); // Return failure response for AJAX

                // Return the JSON response
                return Json(new { success = true, entityExists = dto.IsSIExist, message = "Invoice Generated Successfully" });
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
