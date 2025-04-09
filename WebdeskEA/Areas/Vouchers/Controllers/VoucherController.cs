using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using WebdeskEA.ViewModels;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace CRM.Areas.Accounts.Controllers

{
    [Area("Vouchers")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class VoucherController : Controller
    {
        //=================== Constructor =======================
        #region Constructor
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IPIRepository _PIRepository;
        private readonly ISIRepository _SIRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IVoucherDtlRepository _voucherDtlRepository;
        private readonly ICOACategoryRepository _COACategoryRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICOARepository _CoaRepository;
        private readonly ICOATypeRepository _CoaTypeRepository;
        private readonly IEnumService _enumService;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly ICompanyBusinessCategoryRepository _companyBusinessCategoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        public VoucherController(
            IVoucherRepository voucherRepository,
            IVoucherDtlRepository voucherDtlRepository,
            ICompanyUserRepository companyUserRepository,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository,
            ICompanyBusinessCategoryRepository companyBusinessCategoryRepository,
            ICOARepository CoaRepository,
            ICOATypeRepository CoaTypeRepository,
            ICOACategoryRepository COACategoryRepository,
            IEnumService enumService,
            ITenantPrefixService tenantPrefixService,
            IBankRepository bankRepository,
            ICustomerRepository customerRepository,
            ISupplierRepository supplierRepository,
            ISIRepository SIRepository,
            IPIRepository PIRepository
            )
        {
            _PIRepository = PIRepository;
            _SIRepository = SIRepository;
            _bankRepository = bankRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _voucherRepository = voucherRepository;
            _voucherDtlRepository = voucherDtlRepository;
            _companyUserRepository = companyUserRepository;
            _enumService = enumService;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
            _companyBusinessCategoryRepository = companyBusinessCategoryRepository;
            _CoaRepository = CoaRepository;
            _CoaTypeRepository = CoaTypeRepository;
            _COACategoryRepository = COACategoryRepository;
            _tenantPrefixService = tenantPrefixService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Voucher";
            base.OnActionExecuting(context);
        }

        #endregion

        //=================== Model_Binding =====================
        #region Model_Binding
        /*
        =========================================================
        ===================== 1. Cash Receipt Voucher ===========
        =========================================================
        */
        #region 1. CRV
        // voucherType = 1
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> CRVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "CRV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> CRVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CRV);
                    dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CRVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CRV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "CRV";
                    dto.PaidInvoiceType = InvoiceType.SaleInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(CRVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CRV);
                    dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> CRVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> CRVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);

                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> CRVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "CRV";
                    dto.PaidInvoiceType = InvoiceType.SaleInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(CRVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> CRVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("CRVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CRVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        /*
        =======================================================
        =================== 2. Bank Receipt Voucher ===========
        =======================================================
        */
        #region 2. BRV
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> BRVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "BRV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> BRVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.BRV);
                    dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");

                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BRVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.BRV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "BRV";
                    dto.PaidInvoiceType = InvoiceType.SaleInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(BRVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.BRV, true);
                    dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> BRVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        //----- Update -----
        public async Task<IActionResult> BRVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> BRVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "BRV";
                    dto.PaidInvoiceType = InvoiceType.SaleInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                        dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(BRVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> BRVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.SIDtos = await _SIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("CRVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BRVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(BRVIndex));
        }
        #endregion

        //=======================================================
        //=================== 3. Cash Payment Voucher ===========
        //=======================================================
        #region 3. CPV
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> CPVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "CPV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> CPVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CPV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CPVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CPV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "CPV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(CPVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.CPV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> CPVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> CPVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> CPVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "CPV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(CPVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> CPVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("CRVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CPVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        //=======================================================
        //=================== 4. Bank Payment Voucher ===========
        //=======================================================
        #region 4. BPV
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> BPVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "BPV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> BPVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.BPV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);

                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BPVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.BPV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "BPV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(BPVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.BPV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> BPVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> BPVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> BPVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "BPV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                        dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(BPVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> BPVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("CRVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BPVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(BPVIndex));
        }
        #endregion

        //=======================================================
        //=================== 5. Journal Voucher ================
        //=======================================================
        #region 5. JV 
        //----- Get -----  same BPV
        [HttpGet]
        public async Task<IActionResult> JVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "JV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> JVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.JV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);

                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.JV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "JV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(JVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.JV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> JVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> JVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> JVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "JV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                        dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(JVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> JVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("JVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(JVIndex));
        }
        #endregion

        //=======================================================
        //=================== 6. Petty Cash Voucher =============
        //=======================================================
        #region 6. PCV
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> PCVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "PCV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> PCVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.PCV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);

                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PCVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.PCV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "PCV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(PCVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.PCV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> PCVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> PCVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> PCVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "PCV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                        dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(PCVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> PCVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("PCVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PCVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(PCVIndex));
        }
        #endregion


        //=======================================================
        //=================== 7. Expense Voucher ================
        //=======================================================
        #region 7. EV
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> EVIndex()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _voucherRepository.GetAllByTenantCompanyIdByVoucherTypeAsync(TenantId, CompanyId, "EV");
                    return View(dto);
                }
                else
                {
                    var list = "";//await _CoaRepository.GetAllAsync();
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Create -----
        [HttpGet]
        public async Task<IActionResult> EVCreate()
        {
            try
            {
                var dto = new VoucherDto();
                if (TenantId > 0)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.EV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);

                }
                else { }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Accounts", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EVCreate(VoucherDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.EV);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "EV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var newCompanyId = await _voucherRepository.AddTransactionAsync(dto);
                    return RedirectToAction(nameof(EVIndex));
                }
                else
                {
                    dto.VoucherCode = await _tenantPrefixService.GetMaxVoucherCodeByParentIdTenantIdVoucherCodeAsync(TenantId, CompanyId, PrefixType.EV);
                    dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                    dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                    dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                    dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Detail -----
        public async Task<IActionResult> EVDetails(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Update -----
        public async Task<IActionResult> EVEdit(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> EVEdit(int id, VoucherDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    dto.VoucherTypeId = 1;
                    dto.VoucherType = "EV";
                    dto.PaidInvoiceType = InvoiceType.PurchaseInvoice.ToString();
                    dto.FinancialYearId = FinancialYearId;
                    var result = await _voucherRepository.UpdateTransactionAsync(dto);
                    if (dto == null)
                    {
                        dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "Liabilities & Credit Cards");
                        dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                        dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                        dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                        dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                        dto.PIDtos = await _PIRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        return NotFound();
                    }
                    return RedirectToAction(nameof(EVIndex));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> EVDelete(int id)
        {
            try
            {
                VoucherDto dto = await _voucherRepository.GetByIdAsync(id, TenantId, CompanyId);
                dto.CustomerDtos = await _customerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COADtos = await _CoaRepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId);
                dto.BankDtos = await _bankRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.VoucherDtlDtos = await _voucherDtlRepository.GetAllByVoucherIdCompanyAndTenantIdAsync(id, TenantId, CompanyId);
                dto.COATypeDtos = await _CoaTypeRepository.GetAllListAsync();
                dto.PaymentTypeList = new SelectList(_enumService.GetPaymentTypes(), "Key", "Value");
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost, ActionName("EVDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EVDeleteConfirmed(int id)
        {
            var result = await _voucherRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(EVIndex));
        }
        #endregion

        #endregion

        //=================== Helper Method =====================
        #region HelperMethods
        private async Task<string> GenerateAccountCode(int accountTypeId, string ParentLevelCode, bool isNewLevel)
        {
            string rtnCode = string.Empty;
            string MaxCode = await _tenantPrefixService.GetMaxAccountCodeByTenantId(TenantId, accountTypeId);
            if (MaxCode == null || String.IsNullOrEmpty(MaxCode))
            {
                rtnCode = $"0{accountTypeId.ToString()}0001";
            }
            else
            {
                if (!isNewLevel)
                {
                    long oldCodeNum = Convert.ToInt64(MaxCode);
                    oldCodeNum += 1;
                    string newCode = oldCodeNum.ToString().PadLeft(MaxCode.Length, '0');
                    rtnCode = newCode;
                }
                else
                {
                    rtnCode = $"{ParentLevelCode}0001";
                }
            }
            return rtnCode;
        }

        //[HttpGet]
        //public IActionResult GetLevelNo(int parentAccountId, int coatypeId)
        //{
        //    // Simulate fetching level value from database or service
        //    VoucherDto Dto = new VoucherDto();
        //    if (parentAccountId > 0)
        //    {
        //        var DtoResult = _CoaRepository.GetByIdAsync(parentAccountId, TenantId, CompanyId);
        //        if (DtoResult.Result != null)
        //        {
        //            Dto = DtoResult.Result;
        //        }
        //        int newLevelNo = Dto.LevelNo == null ? 1 : Dto.LevelNo.Value + 1;
        //        string accountCode = GenerateAccountCode(Dto.CoatypeId.Value, Dto.AccountCode, true).Result;
        //        return Json(new { LevelNo = newLevelNo, accountCode = accountCode }); // Default value if not found
        //    }
        //    else
        //    {
        //        string accountCode = GenerateAccountCode(coatypeId, string.Empty, false).Result;
        //        return Json(new { LevelNo = 1, accountCode = accountCode });
        //    }
        //}
        //[HttpGet]
        //public IActionResult GetParentCOAByCOATypeId(int coatypeId)
        //{
        //    var parentCOAResult = _CoaRepository.GetAllByCompanyIdOrAccountTypeIdAsync(TenantId, CompanyId, coatypeId);
        //    List<VoucherDto> parentCOA = new List<VoucherDto>();
        //    if (parentCOAResult.Result != null)
        //    {
        //        parentCOA = parentCOAResult.Result.ToList();
        //    }
        //    string accountCode = GenerateAccountCode(coatypeId, string.Empty, false).Result;
        //    return Json(new { parentCOA = parentCOA, accountCode = accountCode }); // Default value if not found
        //}
        #endregion

        //=================== Partial Binding ===================
        #region Partial_Binding
        #endregion

        //=================== Error Logs ========================
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
