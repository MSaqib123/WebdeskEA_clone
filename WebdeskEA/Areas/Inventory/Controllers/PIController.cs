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
    public class PIController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IImageService _imageService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPODtlRepository _PODtlRepository;
        private readonly IPIDtlRepository _PIDtlRepository;
        private readonly IPRRepository _PRRepository;
        private readonly IPIRepository _PIRepository;
        private readonly IPIDtlTaxRepository _PIDtlTaxRepository;
        private readonly IPIVATBreakdownRepository _PIVATBreakdownRepository;
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
        private readonly IStockLedgerRepository _stockLedgerRepository;
        private readonly IVoucherRepository _voucherRepository;

        public PIController(
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
            IPIDtlRepository PIDtlRepository,
            IPIRepository PIRepository,
            IPORepository PORepository,
            IPRRepository PRRepository,
            ITenantPrefixService tenantPrefixService,
            IPIVATBreakdownRepository PIVATBreakdownRepository,
            IPIDtlTaxRepository PIDtlTaxRepository,
            ICustomerRepository customerRepository,
            IImageService imageService,
            ICompanyRepository companyRepository,
            IStockLedgerRepository stockLedgerRepository,
            IVoucherRepository voucherRepository)
        {
            _PIDtlTaxRepository = PIDtlTaxRepository;
            _PIVATBreakdownRepository = PIVATBreakdownRepository;
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
            _PIDtlRepository = PIDtlRepository;
            _PIRepository = PIRepository;
            _PRRepository = PRRepository;
            _PORepository = PORepository;
            _tenantPrefixService = tenantPrefixService;
            _stockLedgerRepository = stockLedgerRepository;
            _customerRepository = customerRepository;
            _imageService = imageService;
            _companyRepository = companyRepository;
            _voucherRepository = voucherRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Purchase Invoice";
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
                IEnumerable<PIDto> dto;
                if (TenantId > 0)
                {
                    dto = await _PIRepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _PIRepository.GetAllAsync();
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
                PIDto dto = new PIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
                dto.PICode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.PI, true);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        private async void InsertVoucher(string PICode, PIDto dto)
        {
            VoucherDto voucherDto = new VoucherDto();
            voucherDto.Active = true;
            voucherDto.CreatedBy = User.Identity.Name;
            voucherDto.CreatedOn = DateTime.Now;
            voucherDto.VoucherDate = DateTime.Now;
            voucherDto.VoucherType = "PV";
            voucherDto.VoucherTypeId = 7;
            voucherDto.TenantId = TenantId;
            voucherDto.TenantCompanyId = CompanyId;
            voucherDto.VoucherInvoiceNo = PICode;
            voucherDto.FinancialYearId = FinancialYearId;
            voucherDto.VoucherNarration = "Purchase Voucher Code:" + PICode;
            voucherDto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.PV);
            List<VoucherDtlDto> voucherDtls = new List<VoucherDtlDto>();
            var supp = _supplierRepository.GetByIdAsync(dto.SupplierId);
            VoucherDtlDto dtlobjCust = new VoucherDtlDto();
            dtlobjCust.COAId = supp.Result.COAId;
            dtlobjCust.PaidInvoiceNo = PICode;
            dtlobjCust.Remarks = "Purchase Voucher Code:" + PICode;
            decimal suppDue = dto.PIDtlDtos.Sum(x => x.PIDtlTotalAfterVAT);
            dtlobjCust.CrAmount = suppDue;
            dtlobjCust.DbAmount = 0;
            dtlobjCust.PaidInvoiceType = "Purchase Invoice";
            dtlobjCust.TenantId = voucherDto.TenantId;
            dtlobjCust.TenantCompanyId = voucherDto.TenantCompanyId;
            voucherDtls.Add(dtlobjCust);
            if (dto.PIDtlDtos != null && dto.PIDtlDtos.ToList().Count > 0)
            {

                foreach (var prod in dto.PIDtlDtos)
                {
                    var prodCOA = _productCOARepository.GetSelectedBuyCOAsByProductIdAsync(prod.ProductId).Result.ToList();
                    if (prodCOA != null && prodCOA.Count > 0)
                    {
                        decimal divprice = prod.PIDtlTotal / prodCOA.Count;
                        foreach (var item in prodCOA)
                        {
                            VoucherDtlDto dtlobj = new VoucherDtlDto();
                            dtlobj.COAId = item;
                            dtlobj.PaidInvoiceNo = PICode;
                            dtlobj.CrAmount = 0;
                            dtlobj.DbAmount = divprice;
                            dtlobj.PaidInvoiceType = "Purchase Invoice";
                            dtlobj.Remarks = "Product Purchase Entry for Purchase Voucher Code:" + PICode;
                            dtlobj.TenantId = voucherDto.TenantId; ;
                            dtlobj.TenantCompanyId = voucherDto.TenantCompanyId; ;
                            voucherDtls.Add(dtlobj);
                        }
                    }

                }
                if (dto.PIDtlTaxDtos != null && dto.PIDtlTaxDtos.ToList().Count > 0)
                {
                    foreach (var tax in dto.PIDtlTaxDtos)
                    {

                        var taxCOA = _taxMasterRepository.GetByIdTenantId(tax.TaxId, voucherDto.TenantId).Result;
                        if (taxCOA != null)
                        {

                            VoucherDtlDto dtlobjTax = new VoucherDtlDto();
                            dtlobjTax.COAId = taxCOA.COAId;
                            dtlobjTax.PaidInvoiceNo = PICode;
                            dtlobjTax.CrAmount = 0;
                            dtlobjTax.DbAmount = tax.TaxAmount;
                            dtlobjTax.PaidInvoiceType = "Purchase Invoice";
                            dtlobjTax.Remarks = "Product Purchase VAT Entry for Purchase Voucher Code:" + PICode;
                            dtlobjTax.TenantId = voucherDto.TenantId; ;
                            dtlobjTax.TenantCompanyId = voucherDto.TenantCompanyId;
                            voucherDtls.Add(dtlobjTax);
                        }
                    }
                }

            }
            voucherDto.VoucherDtlDtos = voucherDtls;
            await _voucherRepository.AddTransactionAsync(voucherDto);

        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PIDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PIRepository.AddTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        if (dto != null && dto.PIDtlDtos != null)
                        {
                            string piCode = _PIRepository.GetByIdAsync(id).Result.PICode;
                            foreach (var item in dto.PIDtlDtos)
                            {
                                _stockLedgerRepository.StockLedger(item.ProductId, item.PIDtlQty, 0, piCode, "Purchase Invoice", TenantId, CompanyId, FinancialYearId);
                            }
                        }
                        InsertVoucher(dto.PICode, dto);
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Create);
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
                var dto = await _PIRepository.GetByIdAsync(id) ?? new PIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(id);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlTaxDtos = await _PIDtlTaxRepository.GetAllByPIIdAsync(id);
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

        // PIST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PIDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _PIRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        //Insert Stock Ledger
                        if (dto != null && dto.PIDtlDtos != null)
                        {
                            await _stockLedgerRepository.StockLedger_DeleteByInvoiceCode(TenantId, CompanyId, dto.PICode);
                            foreach (var item in dto.PIDtlDtos)
                            {
                                _stockLedgerRepository.StockLedger(item.ProductId, item.PIDtlQty, 0, dto.PICode, "Purchase Invoice", TenantId, CompanyId, FinancialYearId);
                            }
                        }
                        //End
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update purchase invoice.");
                    }
                }
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
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
                var dto = await _PIRepository.GetByIdAsync(id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(id);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlTaxDtos = await _PIDtlTaxRepository.GetAllByPIIdAsync(id);
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

        // PIST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dto = await _PIRepository.GetByIdAsync(id);
                if (dto == null)
                    return NotFound();

                var (isSuccess, message) = await _PIRepository.ForceDeletePIAsync(id);
                if (isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(id);
                    dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
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
                var dto = await _PIRepository.GetByIdAsync(id) ?? new PIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(id);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlTaxDtos = await _PIDtlTaxRepository.GetAllByPIIdAsync(id);
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
                var dto = await _PIRepository.GetByIdAsync(id) ?? new PIDto();
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(dto.Id);
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(TenantId, CompanyId, FinancialYearId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PODtos = await _PORepository.GetAllNotInUsedByTenantCompanyIdAsync(TenantId, CompanyId, TypeView.Edit, dto.POId ?? 0);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlTaxDtos = await _PIDtlTaxRepository.GetAllByPIIdAsync(id);
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

        #region Partial_Binding

        //====================================================
        //======== For Other Controller | Views ==============
        //====================================================
        #region For Other Controller | Views 
        /// <summary>
        /// this is being used in PR  on  PI Drpdown to fetch the PI list in PR
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetPIById(int id)
        {
            try
            {
                var dto = await _PIRepository.GetByIdAsync(id) ?? new PIDto();
                dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(id);
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

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
        //PI -> Create Copy for SR

        [HttpGet]
        public async Task<IActionResult> CheckPRExists(int piId)
        {
            try
            {
                var dto = await _PIRepository.GetByIdAsync(piId) ?? new PIDto();
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
        public async Task<IActionResult> GotoEditPR(int piId)
        {
            try
            {
                var dto = await _PRRepository.GetByPIIdAsync(piId) ?? new PRDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                if (dto.PIId > 0)
                {
                    var editUrl = Url.Action("Edit", "PR", new { id = dto.Id, area = "Inventory" });
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
        public async Task<IActionResult> GotoDetailPR(int piId)
        {
            try
            {
                var dto = await _PRRepository.GetByPIIdAsync(piId) ?? new PRDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." });

                if (dto.PIId > 0)
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

        public class GeneratePIRequest
        {
            public int PiId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePR([FromBody] GeneratePIRequest request)
        {
            try
            {

                var Result = await _PRRepository.GetByPIIdAsync(request.PiId) ?? new PRDto();
                if (Result.Id > 0)
                    return Json(new { success = false, message = "Invoice Alredy Generated." });

                var dto = await _PIRepository.GetByIdAsync(request.PiId) ?? new PIDto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.FinancialYearId = FinancialYearId;
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(dto.Id);
                dto.PIDtlTaxDtos = await _PIDtlTaxRepository.GetAllByPIIdAsync(dto.Id);
                dto.PIVATBreakdownDtos = await _PIVATBreakdownRepository.GetAllByPIIdAsync(dto.Id);
                dto.PICode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SR, true);
                var result = await _PIRepository.GeneratePRByPI(dto);
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
        public async Task<IActionResult> DeletePR([FromBody] GeneratePIRequest request)
        {
            try
            {
                var dto = await _PIRepository.GetByIdAsync(request.PiId) ?? new PIDto();
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                dto.PIDtlDtos = await _PIDtlRepository.GetAllByPIIdAsync(dto.Id);

                var result = await _PRRepository.DeleteAsync(request.PiId);
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
