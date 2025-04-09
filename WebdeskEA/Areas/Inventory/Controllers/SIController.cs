using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class SIController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IPdfGeneratorService _pdfGeneratorService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISIDtlRepository _SIDtlRepository;
        private readonly ISIVATBreakdownRepository _SIVATBreakdownRepository;
        private readonly ISIDtlTaxRepository _SIDtlTaxRepository;
        private readonly ISIRepository _SIRepository;
        private readonly ISRRepository _SRRepository;
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
        private readonly IStockLedgerRepository _stockLedgerRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IImageService _imageService;
        public SIController(
            ISIVATBreakdownRepository SIVATBreakdownRepository,
            ISIDtlTaxRepository SIDtlTaxRepository,
            ISRRepository SRRepository,
            ICustomerRepository customerRepository,
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
            ISIDtlRepository sODtlRepository,
            ISIRepository sIRepository,
            ISORepository SORepository,
            ITenantPrefixService tenantPrefixService,
            IPdfGeneratorService pdfGeneratorService,
            IStockLedgerRepository stockLedgerRepository,
            IImageService imageService,
            ICompanyRepository companyRepository)
        {
            _SIVATBreakdownRepository = SIVATBreakdownRepository;
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
            _SIDtlRepository = sODtlRepository;
            _SIRepository = sIRepository;
            _SORepository = SORepository;
            _tenantPrefixService = tenantPrefixService;
            _stockLedgerRepository = stockLedgerRepository;
            _SRRepository = SRRepository;
            _SIDtlTaxRepository = SIDtlTaxRepository;
            _pdfGeneratorService = pdfGeneratorService;
            _companyRepository = companyRepository;
            _imageService = imageService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Sale Invoice";
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
                IEnumerable<SIDto> dto;
                if (TenantId > 0)
                {
                    dto = await _SIRepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _SIRepository.GetAllAsync();
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
                SIDto dto = new SIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId);  
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
                dto.SICode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.SI, true);

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
        public async Task<IActionResult> Create(SIDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _SIRepository.AddTransactionAsync(dto);
                 
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        if (dto != null && dto.SIDtlDtos != null)
                        {
                            string siCode = _SIRepository.GetByIdAsync(id).Result.SICode;
                            foreach (var item in dto.SIDtlDtos)
                            {
                                _stockLedgerRepository.StockLedger(item.ProductId, 0, item.SIDtlQty, siCode, "Sale Invoice", TenantId, CompanyId, FinancialYearId);
                            }
                        }
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
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
                var dto = await _SIRepository.GetByIdAsync(id) ?? new SIDto();
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId??0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtlTaxDtos = await _SIDtlTaxRepository.GetAllBySIIdAsync(id);
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
        public async Task<IActionResult> Edit(SIDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _SIRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        if (dto != null && dto.SIDtlDtos != null)
                        {
                            await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId,CompanyId,dto.SICode);
                            foreach (var item in dto.SIDtlDtos)
                            {
                                _stockLedgerRepository.StockLedger(item.ProductId, 0, item.SIDtlQty, dto.SICode, "Sale Invoice", TenantId, CompanyId, FinancialYearId);
                            }
                        }
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update SI.");
                    }
                }
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId ?? 0);
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
                var dto = await _SIRepository.GetByIdAsync(id);
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtlTaxDtos = await _SIDtlTaxRepository.GetAllBySIIdAsync(id);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dto = await _SIRepository.GetByIdAsync(id);
                if (dto == null)
                    return NotFound();

                var (isSuccess, message) = await _SIRepository.ForceDeleteSIAsync(id);
                if (isSuccess)
                {
                    await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId, CompanyId, dto.SICode);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", message);
                    dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                    dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                    dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId ?? 0);
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
                var dto = await _SIRepository.GetByIdAsync(id) ?? new SIDto();
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtlTaxDtos = await _SIDtlTaxRepository.GetAllBySIIdAsync(id);
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
                var dto = await _SIRepository.GetByIdAsync(id) ?? new SIDto();
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SODtos = await _SORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SOId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtlTaxDtos = await _SIDtlTaxRepository.GetAllBySIIdAsync(id);
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
        /// this is being used in SR  on  SI Drpdown to fetch the SI list in SI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetSIById(int id)
        {
            try
            {
                var dto = await _SIRepository.GetByIdAsync(id) ?? new SIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(id);
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
        //SI -> Create Copy for SR

        [HttpGet]
        public async Task<IActionResult> CheckSRExists(int siId)
        {
            try
            {
                var dto = await _SIRepository.GetByIdAsync(siId) ?? new SIDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                return Json(new { success = true, entityExists = dto.IsReturnExist, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpGet]
        public async Task<IActionResult> GotoEditSR(int siId)
        {
            try
            {
                var dto = await _SRRepository.GetBySIIdAsync(siId) ?? new SRDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                if (dto.SIId > 0)
                {
                    var editUrl = Url.Action("Edit", "SR", new { id = dto.Id, area = "Inventory" });
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
        public async Task<IActionResult> GotoDetailSR(int siId)
        {
            try
            {
                var dto = await _SRRepository.GetBySIIdAsync(siId) ?? new SRDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." });

                if (dto.SIId > 0)
                {
                    var editUrl = Url.Action("Details", "SR", new { id = dto.Id, area = "Inventory" });
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
            public int SiId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSR([FromBody] GenerateSIRequest request)
        {
            try
            {

                var Result = await _SRRepository.GetBySIIdAsync(request.SiId) ?? new SRDto();
                if (Result.Id > 0)
                    return Json(new { success = false, message = "Invoice Alredy Generated." });

                var dto = await _SIRepository.GetByIdAsync(request.SiId) ?? new SIDto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.FinancialYearId = FinancialYearId;
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);
                dto.SIDtlTaxDtos = await _SIDtlTaxRepository.GetAllBySIIdAsync(dto.Id);
                dto.SIVATBreakdownDtos = await _SIVATBreakdownRepository.GetAllBySIIdAsync(dto.Id);
                dto.SICode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SR, true);
                var result = await _SIRepository.GenerateSRBySI(dto);
                if (!result)
                    return Json(new { success = false, message = "Invoice not found." });

                return Json(new { success = true, entityExists = dto.IsReturnExist, message = "Invoice Generated Successfully" });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSR([FromBody] GenerateSIRequest request)
        {
            try
            {
                var dto = await _SIRepository.GetByIdAsync(request.SiId) ?? new SIDto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.SIDtlDtos = await _SIDtlRepository.GetAllBySIIdAsync(dto.Id);

                var result = await _SRRepository.DeleteAsync(request.SiId);
                if (result == 0)
                    return Json(new { success = false, message = "salesorder not found." }); // return failure response for ajax

                // Return the JSON response
                return Json(new { success = true, entityExists = dto.IsReturnExist, message = "Invoice Generated Successfully" });
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
