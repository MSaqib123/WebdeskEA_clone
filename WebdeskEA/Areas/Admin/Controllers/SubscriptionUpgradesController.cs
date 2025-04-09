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
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Models.ViewModel;
using WebdeskEA.ViewModels;

namespace WebdeskEA.Areas.Registration.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Tenant Admin,SuperAdmin,Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class SubscriptionUpgradesController : Controller
    {
        //=================== Constructor =======================
        #region Constructor
        private readonly IImageService _imageService;
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
        private readonly ISubscriptionUpgrades_PermisssionsRepository _SubscriptionUpgrades_PermisssionsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebdeskEADBContext _dbContext;


        public SubscriptionUpgradesController(
            ISubscriptionUpgrades_PermisssionsRepository SubscriptionUpgrades_PermisssionsRepository,
            IImageService imageService,
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
            _imageService = imageService;
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
            _SubscriptionUpgrades_PermisssionsRepository = SubscriptionUpgrades_PermisssionsRepository;
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
                dto.CompanyDto.Logo = _imageService.GetImagePath(dto.CompanyDto.Logo, "~/uploads/Company/Logo/") ?? "~/Template/img/160x160/img1.jpg";
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegistrationViewModel model)
        {
            model.ApplicationUserDto.Email = model.ApplicationUserDto.UserName;
            //============== Update Proccess ==========
            #region Regisration_Proccess
            try
            {
                var dto = await GetRegistrationViewModelAsync();

                //========== Upadate Tenant Dapper =========
                dto.TenantDto.TenantName = model.TenantDto.TenantName;
                await _tenantRepository.UpdateAsync(dto.TenantDto);

                //========== Upadate Company =========
                if (model.CompanyDto.CompanyImageForSave != null)
                {
                    _imageService.DeleteImage(dto.CompanyDto!.Logo!, "uploads/Company/Logo");
                    dto.CompanyDto.Logo = _imageService.UploadImage(model.CompanyDto.CompanyImageForSave, "uploads/Company/Logo");
                }
                await _CompanyRepository.UpdateAsync(dto.CompanyDto);

                //========== Update User =========
                dto.ApplicationUserDto.Name = model.TenantDto.TenantName;
                dto.ApplicationUserDto.PhoneNumber = model.ApplicationUserDto.PhoneNumber;
                dto.ApplicationUserDto.StreetAddress = model.ApplicationUserDto.StreetAddress;
                dto.ApplicationUserDto.PostalCode = model.ApplicationUserDto.PostalCode;
                var result = await _applicationUserRepository.UpdateAsync(dto.ApplicationUserDto);
                if (!result.Succeeded)
                {
                    throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                TempData["Success"] = "Updated Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                model.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                model.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
                TempData["Error"] = "Registration Failed!";
                await LogError(ex);
                return View(model);
            }


            #endregion
        }
        #endregion

        //=================== Partial Binding ===================
        #region Partial_Binding

        [HttpGet]
        public JsonResult GetStatesByCountry(int countryId)
        {
            var states = _stateProvinceRepository.GetStateProvinceByCountryIdAsync(countryId)
                .Result
                .Where(s => s.CountryId == countryId)
                .Select(s => new { s.Id, s.StateProvinceName })
                .ToList();
            return Json(states);
        }

        [HttpGet]
        public JsonResult GetCitiesByState(int stateId)
        {
            var cities = _cityRepository.GetCityByStateIdAsync(stateId)
                .Result
                .Where(c => c.StateProvinceId == stateId)
                .Select(c => new { c.Id, c.CityName })
                .ToList();
            return Json(cities);
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
            RegistrationViewModel dto = new RegistrationViewModel();
            dto.ApplicationUserDto = await _applicationUserRepository.GetByIdAsync(userId);
            dto.TenantDto = await _tenantRepository.GetByIdAsync(dto.ApplicationUserDto.TenantId.Value);
            dto.CompanyDto = await _CompanyRepository.GetByIdAsync(dto.ApplicationUserDto.CompanyId.Value);
            dto.SubscriptionUpgrades_PermisssionsDto = await _SubscriptionUpgrades_PermisssionsRepository.GetByUserIdAsync(userId);

            dto.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
            dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
            dto.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
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
