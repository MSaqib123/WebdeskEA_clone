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
using WebdeskEA.ViewModels;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace CRM.Areas.Accounts.Controllers

{
    [Area("Accounts")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class ChartofAccountController : Controller
    {
        //=================== Constructor =====================
        #region Constructor
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        private readonly ICOACategoryRepository _COACategoryRepository;
        private readonly ICOARepository _CoaRepository;
        private readonly ICOATypeRepository _CoaTypeRepository;
        private readonly IEnumService _enumService;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly ICompanyBusinessCategoryRepository _companyBusinessCategoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        public ChartofAccountController(ICompanyUserRepository companyUserRepository,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository,
            ICompanyBusinessCategoryRepository companyBusinessCategoryRepository,
            ICOARepository Coa,
            ICOATypeRepository CoaType,
            ICOACategoryRepository COACategoryRepository,
            IEnumService enumService,
            ITenantPrefixService tenantPrefixService
            )
        {
            _companyUserRepository = companyUserRepository;
            _enumService = enumService;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
            _companyBusinessCategoryRepository = companyBusinessCategoryRepository;
            _CoaRepository = Coa;
            _CoaTypeRepository = CoaType;
            _COACategoryRepository = COACategoryRepository;
            _tenantPrefixService = tenantPrefixService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Chart of Account";
            base.OnActionExecuting(context);
        }

        #endregion

        //=================== Model_Binding =====================
        #region Model_Binding
        //----- Get -----
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (TenantId > 0)
                {
                    var entity = await _CoaRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    return View(entity);
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
        public async Task<IActionResult> Create(int id)
        {
            try
            {
                var dto = new COADto();
                //dto.COADtoList = await _CoaRepository.GetAllAsync();
                if (TenantId > 0)
                {
                    dto.CoatypeId = 1;
                    dto.AccountCode = await GenerateAccountCode(dto.CoatypeId.Value, string.Empty, false);
                    dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.COA, true);
                    dto.COADtoList = await _CoaRepository.GetAllByCompanyIdOrAccountTypeIdAsync(TenantId, CompanyId, dto.CoatypeId.Value);
                    dto.LevelNo = 1;
                    dto.Transable = true;
                }
                else
                {
                    //  dto.COADtoList = await _CoaRepository.GetAllAsync();
                }
                dto.CoatypeDtoList = await _CoaTypeRepository.GetAllListAsync();
                dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");

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
        public async Task<IActionResult> Create(COADto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId,CompanyId, "", PrefixType.COA, true);
                    dto.TenantCompanyId = CompanyId;
                    dto.TenantId = TenantId;
                    var newCompanyId = await _CoaRepository.AddAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (TenantId > 0)
                    {
                        dto.COADtoList = await _CoaRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    }
                    else
                    {
                        //  dto.COADtoList = await _CoaRepository.GetAllAsync();
                    }
                    //Dto.BusinessCategoryList = await _companyBusinessCategoryRepository.GetAllCompanyBusinessCategoryAsync();
                    dto.CoatypeDtoList = await _CoaTypeRepository.GetAllListAsync();
                    dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Account", statusCode = 500, errorMessage = ex.Message });
            }
        }

        //----- Delete -----
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                COADto Dto = await _CoaRepository.GetByIdAsync(id, TenantId, CompanyId);
                return View(Dto);
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
            var result = await _CoaRepository.DeletesAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        //----- Detail -----
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                COADto Dto = await _CoaRepository.GetByIdAsync(id, TenantId, CompanyId);
                return View(Dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        //----- Update -----
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                COADto Dto = await _CoaRepository.GetByIdAsync(id, TenantId, CompanyId);
                if (Dto == null)
                {
                    return NotFound();
                }
                Dto.COADtoList = await _CoaRepository.GetAllAsync();
                //Dto.BusinessCategoryList = await _companyBusinessCategoryRepository.GetAllCompanyBusinessCategoryAsync();
                Dto.CoatypeDtoList = await _CoaTypeRepository.GetAllListAsync();
                Dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                return View(Dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, COADto Dto)
        {
            try
            {
                if (id != Dto.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    Dto.TenantCompanyId = CompanyId;
                    Dto.TenantId = TenantId;
                    var result = await _CoaRepository.UpdateAsync(Dto);
                    if (Dto == null)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                Dto.COADtoList = await _CoaRepository.GetAllAsync();
                //Dto.BusinessCategoryList = await _companyBusinessCategoryRepository.GetAllCompanyBusinessCategoryAsync();
                Dto.CoatypeDtoList = await _CoaTypeRepository.GetAllListAsync();
                Dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");
                return View(Dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        private async Task<string> GenerateAccountCode(int accountTypeId, string ParentLevelCode, bool isNewLevel,int? parentAccount = 0)
        {
            string rtnCode = string.Empty;
            string MaxCode = await _tenantPrefixService.GetMaxAccountCodeByTenantId(TenantId, accountTypeId);
            if (MaxCode == null || String.IsNullOrEmpty(MaxCode))
            {
                rtnCode = $"0{accountTypeId.ToString()}01";
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
                    string parentCode = await _tenantPrefixService.GetMaxAccountCodeByParentIdTenantId(TenantId, accountTypeId,parentAccount.Value);
                    if (parentCode == null || String.IsNullOrEmpty(parentCode))
                    {
                        rtnCode = $"{ParentLevelCode}0001";
                    }
                    else
                    {
                        long oldCodeNum = Convert.ToInt64(parentCode);
                        oldCodeNum += 1;
                        string newCode = oldCodeNum.ToString().PadLeft(parentCode.Length, '0');
                        rtnCode = newCode;
                    }
                }
            }
            return rtnCode;
        }
        [HttpGet]
        public IActionResult GetLevelNo(int parentAccountId, int coatypeId)
        {
            // Simulate fetching level value from database or service
            COADto Dto = new COADto();
            if (parentAccountId > 0)
            {
                var DtoResult = _CoaRepository.GetByIdAsync(parentAccountId, TenantId, CompanyId);
                if (DtoResult.Result != null)
                {
                    Dto = DtoResult.Result;
                }
                int newLevelNo = Dto.LevelNo == null ? 1 : Dto.LevelNo.Value + 1;
                string accountCode = GenerateAccountCode(Dto.CoatypeId.Value, Dto.AccountCode, true).Result;
                return Json(new { LevelNo = newLevelNo, accountCode = accountCode }); // Default value if not found
            }
            else
            {
                string accountCode = GenerateAccountCode(coatypeId, string.Empty, false).Result;
                return Json(new { LevelNo = 1, accountCode = accountCode });
            }
        }
        [HttpGet]
        public IActionResult GetParentCOAByCOATypeId(int coatypeId)
        {
            var parentCOAResult = _CoaRepository.GetAllByCompanyIdOrAccountTypeIdAsync(TenantId, CompanyId, coatypeId);
            List<COADto> parentCOA = new List<COADto>();
            if (parentCOAResult.Result != null)
            {
                parentCOA = parentCOAResult.Result.ToList();
            }
            string accountCode = GenerateAccountCode(coatypeId, string.Empty, false).Result;
            return Json(new { parentCOA = parentCOA, accountCode = accountCode }); // Default value if not found
        }
        #endregion

        //=================== Account_Tree =====================
        #region TreeAccounts
        [HttpGet]
        public async Task<IActionResult> TreeAccounts()
        {
            try
            {
                var dto = new COADto();
                //dto.COADtoList = await _CoaRepository.GetAllAsync();
                if (TenantId > 0)
                {
                    dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.COA, true);
                    dto.CoatypeId = 1;
                    dto.AccountCode = await GenerateAccountCode(dto.CoatypeId.Value, string.Empty, false);
                    dto.COADtoList = await _CoaRepository.GetAllByCompanyIdOrAccountTypeIdAsync(TenantId, CompanyId, dto.CoatypeId.Value);
                    dto.LevelNo = 1;
                    dto.Transable = true;
                }
                else
                {
                    //  dto.COADtoList = await _CoaRepository.GetAllAsync();
                }
                dto.CoaCategoryDtoList = await _COACategoryRepository.GetAllListAsync();
                dto.CoatypeDtoList = await _CoaTypeRepository.GetAllListAsync();
                dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value");

                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoaAndCategories(int coatypeId, int tenantId, int tenantCompanyId)
        {
            //var cats = await _COACategoryRepository.GetAllListAsync();
            //var relevantCats = cats.Where(c => c.CoaTypeId == coatypeId).ToList();

            //var coas = await _CoaRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            //var relevantCoas = coas.Where(c => c.CoatypeId == coatypeId).ToList();

            var allCategories = await GetAllCoaCategories();
            allCategories = allCategories.Where(c => c.CoaTypeId == coatypeId).ToList();
            var allCOAs = await GetAllCOAs();
            allCOAs = allCOAs.ToList();
            var tree = BuildCategoryHierarchy(allCategories, allCOAs);

            return Json(new
            {
                categories = allCategories,
                coas = allCOAs
            });
        }


        [HttpGet]
        public async Task<IActionResult> LoadTree(int coatypeId, int tenantId, int tenantCompanyId)
        {
            var allCategories = await GetAllCoaCategories();
            allCategories = allCategories.Where(c => c.CoaTypeId == coatypeId).ToList();
            var allCOAs = await GetAllCOAs();
            allCOAs = allCOAs.ToList();
            var tree = BuildCategoryHierarchy(allCategories, allCOAs);

            return PartialView("_COATreePartial", tree);
        }


        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateCOA(int? id, int coatypeId, int tenantId, int tenantCompanyId, int? coaCategoryId = null, int? parentAccountId = null)
        {
            var dto = new COADto
            {
                CoatypeId = coatypeId,
                TenantId = tenantId,
                TenantCompanyId = tenantCompanyId,
                CoaCategoryId = coaCategoryId ?? 0,
                ParentAccountId = parentAccountId ?? 0,
                Transable = true
            };
            if (id.HasValue && id.Value > 0)
            {
                // load existing from DB
                var existing = GetAllCOAs().Result.FirstOrDefault(x => x.Id == id.Value);
                if (existing != null) dto = existing;
            }
            var DtoP = new COADto();
            string parentCode = string.Empty;
            bool isNewLevel = false;
            if (parentAccountId.HasValue && parentAccountId.Value > 0) {
                var DtoResult = _CoaRepository.GetByIdAsync(parentAccountId.Value, TenantId, CompanyId);
                if (DtoResult.Result != null)
                {
                    DtoP = DtoResult.Result;
                }
                int newLevelNo = DtoP.LevelNo == null ? 1 : DtoP.LevelNo.Value + 1;
                parentCode = DtoP.AccountCode;
                isNewLevel = true;
                dto.LevelNo = newLevelNo;
                //string accountCode = GenerateAccountCode(DtoP.CoatypeId.Value, DtoP.AccountCode, true).Result;
            }
            else
            {
                dto.LevelNo = 1;
            }
           
            dto.AccountCode = await GenerateAccountCode(dto.CoatypeId.Value, parentCode, isNewLevel,parentAccountId);
            dto.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.COA, true);
            dto.CoaTransactionTypeList = new SelectList(_enumService.GetTransactionTypes(), "Key", "Value", dto.CoaTranType);

            return PartialView("_COAModalPartial", dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateCOA([FromBody] COADto model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");
            model.TenantId = TenantId;
            model.TenantCompanyId = CompanyId;
            var DtoP = new COADto();
            string parentCode = string.Empty;
            bool isNewLevel = false;
            if (model.ParentAccountId.HasValue && model.ParentAccountId.Value > 0)
            {
                var DtoResult = _CoaRepository.GetByIdAsync(model.ParentAccountId.Value, TenantId, CompanyId);
                if (DtoResult.Result != null)
                {
                    DtoP = DtoResult.Result;
                }
                int newLevelNo = DtoP.LevelNo == null ? 1 : DtoP.LevelNo.Value + 1;
                parentCode = DtoP.AccountCode;
                isNewLevel = true;
                model.LevelNo = newLevelNo;
                //string accountCode = GenerateAccountCode(DtoP.CoatypeId.Value, DtoP.AccountCode, true).Result;
            }
            else
            {
                model.LevelNo = 1;
            }
            model.AccountCode = await GenerateAccountCode(model.CoatypeId.Value, parentCode, isNewLevel,model.ParentAccountId);
            //model.AccountCode = await GenerateAccountCode(model.CoatypeId ?? 0, string.Empty, false);
            model.Code = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.COA, true);
            if (model.Id == 0)
            {
                await _CoaRepository.AddAsync(model);
            }
            else
            {
                await _CoaRepository.UpdateAsync(model);
            }
            // Return updated partial
            var cats = await GetAllCoaCategories();
            var relevantCats = cats.Where(c => c.CoaTypeId == model.CoatypeId).ToList();

            var coas = await GetAllCOAs();
            var relevantCoas = coas.Where(c => c.CoatypeId == model.CoatypeId && c.TenantId == TenantId && c.TenantCompanyId == CompanyId).ToList();

            var tree = BuildCategoryHierarchy(relevantCats, relevantCoas);
            return PartialView("_COATreePartial", tree);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateCOACategory(int? id, int coatypeId)
        {
            var dto = new CoaCategoryDto
            {
                CoaTypeId = coatypeId,
            };
            if (id.HasValue && id.Value > 0)
            {
                // load existing from DB
                var existing = GetAllCOAs().Result.FirstOrDefault(x => x.Id == id.Value);
            }
            return PartialView("_CoaCategoryModalPartial", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateCOACategory([FromBody] CoaCategoryDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Invalid data provided." });

            try
            {
                if (model.Id == 0)
                {
                    await _COACategoryRepository.AddAsync(model);
                }
                else
                {
                    await _COACategoryRepository.UpdateAsync(model);
                }
                //return Json(new { status = true, message = "Inserted successfully." });

                var cats = await GetAllCoaCategories();
                var relevantCats = cats.Where(c => c.CoaTypeId == model.CoaTypeId).ToList();
                var coas = await GetAllCOAs();
                var relevantCoas = coas.Where(c => c.CoatypeId == model.CoaTypeId && c.TenantId == TenantId && c.TenantCompanyId == CompanyId).ToList();
                var tree = BuildCategoryHierarchy(relevantCats, relevantCoas);
                return PartialView("_COATreePartial", tree);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary (e.g., using a logging framework)
                // Example: _logger.LogError(ex, "Error in CreateOrUpdateCOACategory");

                return Json(new { status = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCOA([FromBody] DeleteCOARequest request)
        {
            await _CoaRepository.DeletesAsync(request.Id);
            // Return updated partial
            var cats = await GetAllCoaCategories();
            var relevantCats = cats.Where(c => c.CoaTypeId == request.CoatypeId).ToList();

            var coas = await GetAllCOAs();
            var relevantCoas = coas.Where(c => c.CoatypeId == request.CoatypeId && c.TenantId == request.TenantId && c.TenantCompanyId == request.TenantCompanyId).ToList();

            var tree = BuildCategoryHierarchy(relevantCats, relevantCoas);
            return PartialView("_COATreePartial", tree);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalize([FromBody] FinalizeRequest request)
        {
            // finalize logic
            return Json(new { success = true });
        }

        private async Task<List<CoaCategoryDto>> GetAllCoaCategories()
        {
            var list = (await _COACategoryRepository.GetAllListAsync()).ToList();
            return list;
        }

        private async Task<List<COADto>> GetAllCOAs()
        {
            var list = (await _CoaRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId)).ToList();
            return list;
        }

        private List<CoaCategoryHierarchyDto> BuildCategoryHierarchy(List<CoaCategoryDto> categories, List<COADto> coas)
        {
            var hierarchy = new List<CoaCategoryHierarchyDto>();

            foreach (var category in categories)
            {
                var categoryCoAs = coas
                    .Where(c => c.CoaCategoryId == category.Id)
                    .ToList();

                var hierarchicalCoAs = BuildCOAHierarchy(categoryCoAs);

                hierarchy.Add(new CoaCategoryHierarchyDto
                {
                    CoaCategory = category,
                    COADtoList = hierarchicalCoAs
                });
            }

            return hierarchy;
        }

        private List<COADto> BuildCOAHierarchy(List<COADto> coas)
        {
            var lookup = coas.ToDictionary(c => c.Id, c => c);
            var roots = new List<COADto>();
            foreach (var coa in coas)
            {
                if (coa.ParentAccountId.HasValue && lookup.ContainsKey(coa.ParentAccountId.Value))
                {
                    var parent = lookup[coa.ParentAccountId.Value];
                    var tempList = parent.COADtoList?.ToList() ?? new List<COADto>();
                    tempList.Add(coa);
                    parent.COADtoList = tempList;
                }
                else
                {
                    roots.Add(coa);
                }
            }
            return roots;
        }



        public class DeleteCOARequest
        {
            public int Id { get; set; }
            public int CoatypeId { get; set; }
            public int TenantId { get; set; }
            public int TenantCompanyId { get; set; }
        }

        public class FinalizeRequest
        {
            public int TenantId { get; set; }
            public int TenantCompanyId { get; set; }
        }
        #endregion

        //=================== Partial Binding =====================
        #region Partial_Binding
        #endregion

        //=================== Error Logs =====================
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
