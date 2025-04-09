using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;
using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.SecurityService;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Models.ViewModel;
using WebdeskEA.ViewModels;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class ProfileDisplayController : Controller
    {
        //=================== Constructor =======================
        #region Constructor
        private readonly ICOARepository _Coa;
        private readonly ICOATypeRepository _CoaType;
        private readonly IEnumService _enumService;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly ICompanyBusinessCategoryRepository _companyBusinessCategoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateProvinceRepository _stateProvinceRepository;
        private readonly ILoginService _loginService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackagePermissionRepository _packagePermissionRepository;
        private readonly ITenantPermissionRepository _tenantPermissionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebdeskEADBContext _dbContext;


        public ProfileDisplayController(
            ICompanyUserRepository companyUserRepository,
            UserManager<ApplicationUser> userManager,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository,
            ICompanyBusinessCategoryRepository companyBusinessCategoryRepository,
            ICOARepository Coa,
            ICOATypeRepository CoaType,
            IEnumService enumService,
            IStateProvinceRepository stateProvinceRepository,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            ILoginService loginService,
            IApplicationUserRepository applicationUserRepository,
            ITenantRepository tenantRepository,
            IPackageRepository packageRepository,
            IPackagePermissionRepository packagePermissionRepository,
            ITenantPermissionRepository tenantPermissionRepository,
            WebdeskEADBContext context
            )
        {
            _userManager = userManager;
            _companyUserRepository = companyUserRepository;
            _enumService = enumService;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
            _companyBusinessCategoryRepository = companyBusinessCategoryRepository;
            _Coa = Coa;
            _CoaType = CoaType;
            _dbContext = context;
            _stateProvinceRepository = stateProvinceRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _loginService = loginService;
            _tenantRepository = tenantRepository;
            _applicationUserRepository = applicationUserRepository;
            _packageRepository = packageRepository;
            _packagePermissionRepository = packagePermissionRepository;
            _tenantPermissionRepository = tenantPermissionRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Chart of Account";
            base.OnActionExecuting(context);
        }

        #endregion

        //=================== Model_Binding =====================
        #region Model_Binding
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dto = await GetRegistrationViewModelAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion

        //=================== Partial Binding ===================
        #region Partial_Binding

        [HttpGet]
        public async Task<IActionResult> LoadPartialView(string viewName)
        {
            try
            {
                var dto = await GetRegistrationViewModelAsync();

                switch (viewName)
                {
                    case "Profile":
                        dto.TenantDto = await _tenantRepository.GetByIdAsync(dto.ApplicationUserDto.TenantId ?? 1);
                        return PartialView("_Profile", dto);

                    case "Company":
                        dto.CompanyDto = await _CompanyRepository.GetByIdAsync(dto.ApplicationUserDto.CompanyId ?? 1);
                        return PartialView("_ProfileCompany", dto);

                    default:
                        dto.TenantDto = await _tenantRepository.GetByIdAsync(dto.ApplicationUserDto.TenantId ?? 1);
                        return PartialView("_Profile", dto);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return StatusCode(500, "An error occurred while loading the partial view.");
            }
        }

        #endregion

        //=================== Helper Method ===================
        #region Helper Methods
        private async Task<RegistrationViewModel> GetRegistrationViewModelAsync()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                throw new Exception("User is not authenticated");
            }
            RegistrationViewModel dto = new RegistrationViewModel
            {
                ApplicationUserDto = await _applicationUserRepository.GetByIdAsync(userId)
            };
            return dto;
        }
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
