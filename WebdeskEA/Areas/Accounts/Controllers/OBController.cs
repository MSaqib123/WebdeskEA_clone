using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Accounts")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class OBController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IPIRepository _PIRepository;
        private readonly ICOATypeRepository _CoaTypeRepository;
        private readonly IEnumService _enumService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOBDtlRepository _OBDtlRepository;
        private readonly IOBRepository _OBRepository;
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
        public OBController(
            ICOATypeRepository CoaType,
            IEnumService enumService,
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
            IOBDtlRepository sODtlRepository,
            IOBRepository sORepository,
            ITenantPrefixService tenantPrefixService,
            IPIRepository PIRepository)
        {
            _CoaTypeRepository = CoaType;
            _enumService = enumService;
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
            _OBDtlRepository = sODtlRepository;
            _OBRepository = sORepository;
            _tenantPrefixService = tenantPrefixService;
            _PIRepository = PIRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Opening Balance";
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
                IEnumerable<OBDto> dto;
                if (TenantId > 0)
                {
                    dto = await _OBRepository.GetAllByTenantCompanyFinancialYearIdAsync(TenantId, CompanyId, FinancialYearId);
                }
                else
                {
                    dto = null;// await _OBRepository.GetAllAsync();
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
                OBDto dto = new OBDto();
                dto.OBCode = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.OB, true);
                dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId,CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // OBST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OBDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _OBRepository.AddTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                dto.OBCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.OB, true);
                dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                var dto = await _OBRepository.GetByIdAsync(id) ?? new OBDto();
                dto.OBCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.OB, true);
                dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                dto.COADtos = await _COARepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.OBDtlDtos = await _OBDtlRepository.GetAllByOBIdAsync(id);
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

        // OBST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OBDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    dto.FinancialYearId = FinancialYearId;
                    var id = await _OBRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                //dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                //dto.SupplierDtos = await _supplierRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
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
                var module = await _OBRepository.GetByIdAsync(id);
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

        // OBST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var module = await _OBRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                var result = await _OBRepository.DeleteAsync(id);
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

        //_______ Partial Binding ______
        #region Partial_Binding

        //====================================================
        //======== For Other Controller | Views ==============
        //====================================================
        #region For Other Controller | Views 
        /// <summary>
        /// this is being used in PI  on  OB Drpdown to fetch the SO list in SI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetOBById(int id)
        {
            try
            {
                var dto = await _OBRepository.GetByIdAsync(id) ?? new OBDto();
                //dto.ProductDtos = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                //dto.OBDtlDtos = await _OBDtlRepository.GetAllByOBIdAsync(id);
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
                var dto = await _OBRepository.GetByIdAsync(poId) ?? new OBDto();
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                return Json(new { success = true, entityExists = true, data = dto });
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
            return View();
            //try
            //{
            //    var dto = await _PIRepository.GetByOBIdAsync(poId) ?? new PIDto();
            //    if (dto == null)
            //        return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

            //    if (dto.OBId > 0)
            //    {
            //        var editUrl = Url.Action("Edit", "PI", new { id = dto.Id, area = "Inventory" });
            //        Console.WriteLine(editUrl);
            //        return Json(new { success = true, entityUrl = editUrl, entityExists = true, data = dto });
            //    }
            //    else
            //    {
            //        return Json(new { success = false, entityExists = false, data = dto });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await LogError(ex);
            //    return Json(new { success = false, message = ex.Message }); // Error response in JSON
            //}
        }


        public class GeneratePIRequest
        {
            public int PoId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePI([FromBody] GeneratePIRequest request)
        {
            //try
            //{

            //    var siResult = await _PIRepository.GetByOBIdAsync(request.PoId) ?? new PIDto();
            //    if (siResult.Id > 0)
            //        return Json(new { success = false, message = "Invoice Alredy Generated." });

            //    var dto = await _OBRepository.GetByIdAsync(request.PoId) ?? new OBDto();
            //    dto.TenantId = TenantId;
            //    dto.CompanyId = CompanyId;
            //    dto.OBDtlDtos = await _OBDtlRepository.GetAllByOBIdAsync(dto.Id);
            //    dto.OBCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, "", PrefixType.PI, true);
            //    var result = await _OBRepository.GeneratePIByOB(dto);
            //    if (!result)
            //        return Json(new { success = false, message = "Invoice not found." });

            //    return Json(new { success = true, entityExists = dto.IsPIExist, message = "Invoice Generated Successfully" });
            //}
            //catch (Exception ex)
            //{
            //    await LogError(ex);
            //    return Json(new { success = false, message = ex.Message }); // Error response in JSON
            //}
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePI([FromBody] GeneratePIRequest request)
        {
            //try
            //{
            //    var dto = await _OBRepository.GetByIdAsync(request.PoId) ?? new OBDto();
            //    dto.TenantId = TenantId;
            //    dto.CompanyId = CompanyId;
            //    dto.OBDtlDtos = await _OBDtlRepository.GetAllByOBIdAsync(dto.Id);

            //    var result = await _OBRepository.DeletePIByOB(request.PoId);
            //    if (!result)
            //        return Json(new { success = false, message = "SalesOrder not found." }); // Return failure response for AJAX

            //    // Return the JSON response
            //    return Json(new { success = true, entityExists = dto.IsPIExist, message = "Invoice Generated Successfully" });
            //}
            //catch (Exception ex)
            //{
            //    await LogError(ex);
            //    return Json(new { success = false, message = ex.Message }); // Error response in JSON
            //}
            return View();
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
