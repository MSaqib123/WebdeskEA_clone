using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
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
    public class PRController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPODtlRepository _PODtlRepository;
        private readonly IPRDtlRepository _PRDtlRepository;
        private readonly IPRRepository _PRRepository;
        private readonly IPORepository _PORepository;
        private readonly IPIRepository _PIRepository;
        private readonly IPIDtlRepository _PIDtlRepository;
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
        private readonly IPRDtlTaxRepository _PRDtlTaxRepository;
        private readonly IImageService _imageService;
        private readonly ICompanyRepository _companyRepository;

        public PRController(
            IPIDtlRepository PIDtlRepository,
            ISIRepository SIRepository,
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
            IPRDtlRepository PRDtlRepository,
            IPRRepository PRRepository,
            IPORepository PORepository,
            IPIRepository PIRepository,
            ITenantPrefixService tenantPrefixService,
            IPRDtlTaxRepository PRDtlTaxRepository,
            IImageService imageService,
            ICompanyRepository companyRepository,
            IStockLedgerRepository stockLedgerRepository)
        {
            _PRDtlTaxRepository = PRDtlTaxRepository;
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
            _PRDtlRepository = PRDtlRepository;
            _PRRepository = PRRepository;
            _PORepository = PORepository;
            _PIRepository = PIRepository;
            _tenantPrefixService = tenantPrefixService;
            _stockLedgerRepository = stockLedgerRepository;
            _PIDtlRepository = PIDtlRepository;
            _companyRepository = companyRepository;
            _imageService = imageService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Purchase Return";
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
                IEnumerable<PRDto> dto;
                if (TenantId > 0)
                {
                    dto = await _PRRepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _PRRepository.GetAllAsync();
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
        public async Task<IActionResult> Create()
        {
            try
            {
                PRDto dto = new PRDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId,TypeView.Create);
                dto.PRCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.PR, true);

                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // PRST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PRDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PRRepository.AddTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        //if (dto != null && dto.PRDtlDtos != null)
                        //{
                        //    string piCode = _PRRepository.GetByIdAsync(id).Result.PRCode;
                        //    foreach (var item in dto.PRDtlDtos)
                        //    {
                        //        _stockLedgerRepository.StockLedger(item.ProductId, item.PRDtlQty,0, piCode, "Purchase Invoice", TenantId, CompanyId, FinancialYearId);
                        //    }
                        //}
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
                dto.PRCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.PR, true);
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
                var dto = await _PRRepository.GetByIdAsync(id) ?? new PRDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlDtos = await _PRDtlRepository.GetAllByPRIdAsync(id);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.PIId??0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlTaxDtos = await _PRDtlTaxRepository.GetAllByPRIdAsync(id);
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

        // PRST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PRDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PRRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        //if (dto != null && dto.PRDtlDtos != null)
                        //{
                        //    await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId, CompanyId, dto.PRCode);
                        //    foreach (var item in dto.PRDtlDtos)
                        //    { 
                        //        _stockLedgerRepository.StockLedger(item.ProductId, item.PRDtlQty, 0, dto.PRCode, "Purchase Invoice", TenantId, CompanyId, FinancialYearId);
                        //    }
                        //}
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update purchase return.");
                    }
                }
                dto.PRDtlDtos = await _PRDtlRepository.GetAllByPRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId,FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.PIId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                var dto = await _PRRepository.GetByIdAsync(id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlDtos = await _PRDtlRepository.GetAllByPRIdAsync(id);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.PIId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlTaxDtos = await _PRDtlTaxRepository.GetAllByPRIdAsync(id);
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

        // PRST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dto = await _PRRepository.GetByIdAsync(id);
                if (dto == null)
                    return NotFound();

                var (isSuccess, message) = await _PRRepository.ForceDeletePRAsync(id);
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
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var dto = await _PRRepository.GetByIdAsync(id) ?? new PRDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlDtos = await _PRDtlRepository.GetAllByPRIdAsync(id);
                dto.PIDtos = await _PIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.PIId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlTaxDtos = await _PRDtlTaxRepository.GetAllByPRIdAsync(id);
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

        // GET: Modules/Detail/{id}
        [HttpGet]
        public async Task<IActionResult> DetailsPrint(int id)
        {
            try
            {
                var dto = await _PRRepository.GetByIdAsync(id) ?? new PRDto();
                dto.PRDtlDtos = await _PRDtlRepository.GetAllByPRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PRDtlTaxDtos = await _PRDtlTaxRepository.GetAllByPRIdAsync(id);
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
