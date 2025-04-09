using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.SqlClient;
using System.Security.Claims;
using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.SecurityService;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Models.ViewModel;
using WebdeskEA.ViewModels;
using static WebdeskEA.Areas.Security.Controllers.SecurityController;
namespace CRM.Areas.Registration.Controllers
{
    [Area("Registration")]
    public class RegistrationController : Controller
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebdeskEADBContext _dbContext;


        public RegistrationController(
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
                RegistrationViewModel dto = new RegistrationViewModel();
                dto.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                dto.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();

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
            var transactionDone = false;
            model.ApplicationUserDto.Email = model.ApplicationUserDto.UserName;
            ModelState.Remove("ApplicationUserDto.Id");

            //============== Check Email,password ==========
            #region Email , Password Checking
            var existingUser = await _userManager.FindByEmailAsync(model.ApplicationUserDto.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                model.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                model.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                model.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
                return View(model);
            }
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, null, model.ApplicationUserDto.Password);
            if (!passwordValidationResult.Succeeded)
            {
                foreach (var error in passwordValidationResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                model.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                model.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
                return View(model);
            }
            #endregion

            //============== Registration_Proccess ==========
            #region Regisration_Proccess
            if (ModelState.IsValid)
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //================= Package ============================
                        var package = await _packageRepository.GetByIdAsync(1);
                        var PackagePermission =await _packagePermissionRepository.GetAllByPackageIdAsync(package.Id);
                        
                        //========== Insert Tenant Dapper =========
                        model.TenantDto.TenantCompanies = package.TotalCompany ?? 1; // Package Company
                        model.TenantDto.TenantUsers = package.TotalUser ?? 1; // Package User
                        model.TenantDto.TenantExpiryDate = DateTime.Now.AddYears(2);
                        model.TenantDto.TenantTypeId = 1;
                        var tenantId = await _tenantRepository.AddAsync(model.TenantDto);

                        //========== Insert TenantPermission Dapper =========
                        foreach (var item in PackagePermission)
                        {
                            TenantPermissionDto tp = new TenantPermissionDto
                            {
                                TenantId = tenantId,
                                ModuleId = item.ModuleId ?? 1,
                                IsModuleActive = true
                            };
                            await _tenantPermissionRepository.AddAsync(tp);
                        }

                        //========== Insert Company Dapper =========

                        if (model.CompanyDto.CompanyImageForSave != null)
                        {
                            model.CompanyDto.Logo = _imageService.UploadImage(model.CompanyDto.CompanyImageForSave, "uploads/Company/Logo");
                        }
                        model.CompanyDto.TenantId = tenantId;
                        model.CompanyDto.Description = "Company" + model.CompanyDto.Name;
                        model.CompanyDto.IsMainCompany = true;
                        var compId = await _CompanyRepository.AddAsync(model.CompanyDto);


                        //========== Insert User =========
                        model.ApplicationUserDto.Name = model.TenantDto.TenantName;
                        model.ApplicationUserDto.CompanyId = compId;
                        model.ApplicationUserDto.roleId = 1;
                        model.ApplicationUserDto.isTenantAdmin = true;
                        model.ApplicationUserDto.TenantId = tenantId;
                        var result = await _applicationUserRepository.AddAsync(model.ApplicationUserDto);
                        if (!result.Succeeded)
                        {
                            throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                        }

                        //====== Success =====
                        await transaction.CommitAsync();
                        transactionDone = true;

                        //====== Login Process =====
                        var loginInputModel = new LoginInputViewModel
                        {
                            Email = model.ApplicationUserDto.UserName,
                            Password = model.ApplicationUserDto.Password,
                            RememberMe = true
                        };
                        var loginResult = await _loginService.LoginAsync(loginInputModel, "/");
                        return loginResult;
                    }
                    catch (Exception ex)
                    {
                        model.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                        model.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                        model.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
                        if (!transactionDone)
                        {
                            await transaction.RollbackAsync();
                        }
                        TempData["Error"] = "Registration Failed!";
                        await LogError(ex);
                        return View(model);
                    }
                }
            }
            #endregion

            model.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
            model.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
            model.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
            return View(model);
        }
        #endregion

        //=================== Partial Binding ===================
        #region Partial_Binding
        public class EmailPasswordDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> CheckEmailAndPassword([FromBody] EmailPasswordDto dto)
        {
            
            bool emailExists = false;
            if (dto.Email.ToLower() == "superadmin@gmail.com".ToLower())
            {
                emailExists = true;
            }
            else
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                emailExists = existingUser != null;
            }
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, null, dto.Password);
            bool passInCorrect = false;
            if (!passwordValidationResult.Succeeded)
            {
                passInCorrect = true;
            }
            return Json(new { exists = emailExists, passInCorrect = passInCorrect });
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
