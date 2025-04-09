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
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class SRController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly ICustomerRepository _customerRepository;
        private readonly ISRDtlTaxRepository _SRDtlTaxRepository;
        private readonly ISRDtlRepository _SRDtlRepository;
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
        public SRController(
            ISRDtlTaxRepository SRDtlTaxRepository,
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
            ISRDtlRepository sODtlRepository,
            ISRRepository sORepository,
            ISIRepository sIRepository,
            ISORepository SORepository,
            ITenantPrefixService tenantPrefixService,
            ICompanyRepository companyRepository,
            IImageService imageService,
            IStockLedgerRepository stockLedgerRepository)
        {
            _SRDtlTaxRepository = SRDtlTaxRepository;
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
            _SRDtlRepository = sODtlRepository;
            _SRRepository = sORepository;
            _SIRepository = sIRepository;
            _SORepository = SORepository;
            _tenantPrefixService = tenantPrefixService;
            _stockLedgerRepository = stockLedgerRepository;
            _companyRepository = companyRepository;
            _imageService = imageService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Sale Return";
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
                IEnumerable<SRDto> dto;
                if (TenantId > 0)
                {
                    dto = await _SRRepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _SRRepository.GetAllAsync();
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
                SRDto dto = new SRDto();
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId,TypeView.Create);
                dto.SRCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.SR, true);
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
        public async Task<IActionResult> Create(SRDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    var id = await _SRRepository.AddTransactionAsync(dto);
                 
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        //if (dto != null && dto.SRDtlDtos != null)
                        //{
                        //    string siCode = _SRRepository.GetByIdAsync(id).Result.SRCode;
                        //    foreach (var item in dto.SRDtlDtos)
                        //    {
                        //        _stockLedgerRepository.StockLedger(item.ProductId, 0, item.SRDtlQty, siCode, "Sale Invoice", TenantId, CompanyId, FinancialYearId);
                        //    }
                        //}
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
                dto.SRCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SR, true);
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
                var dto = await _SRRepository.GetByIdAsync(id) ?? new SRDto();
                dto.SRDtlDtos = await _SRDtlRepository.GetAllBySRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SIId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SRDtlTaxDtos = await _SRDtlTaxRepository.GetAllBySRIdAsync(id);
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
        public async Task<IActionResult> Edit(SRDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _SRRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        //if (dto != null && dto.SRDtlDtos != null)
                        //{
                        //    await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId,CompanyId,dto.SRCode);
                        //    foreach (var item in dto.SRDtlDtos)
                        //    {
                        //        _stockLedgerRepository.StockLedger(item.ProductId, 0, item.SRDtlQty, dto.SRCode, "Sale Invoice", TenantId, CompanyId, FinancialYearId);
                        //    }
                        //}
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update SR.");
                    }
                }
                dto.SRDtlDtos = await _SRDtlRepository.GetAllBySRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId,FinancialYearId); 
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SIId ?? 0);
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
                var dto = await _SRRepository.GetByIdAsync(id);
                dto.SRDtlDtos = await _SRDtlRepository.GetAllBySRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SIId ?? 0);
                dto.SRDtlTaxDtos = await _SRDtlTaxRepository.GetAllBySRIdAsync(id);
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
                var module = await _SRRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                var (isSuccess, message) = await _SRRepository.ForceDeleteSRAsync(id);
                if (isSuccess)
                {
                    await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId, CompanyId, module.SRCode);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", message);
                    return View(module);
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
                var dto = await _SRRepository.GetByIdAsync(id) ?? new SRDto();
                dto.SRDtlDtos = await _SRDtlRepository.GetAllBySRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.SIId ?? 0);
                dto.SRDtlTaxDtos = await _SRDtlTaxRepository.GetAllBySRIdAsync(id);
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
                var dto = await _SRRepository.GetByIdAsync(id) ?? new SRDto();
                dto.SRDtlDtos = await _SRDtlRepository.GetAllBySRIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.SRDtlTaxDtos = await _SRDtlTaxRepository.GetAllBySRIdAsync(id);
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
