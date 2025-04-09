using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa;
using System.Reflection;
using System.Security.Claims;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using static Dapper.SqlMapper;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;
namespace CRM.Areas.Accounts.Controllers
{
    [Area("Common")]
    public class GlobalSettingsController : Controller
    {
        //=================== Constructor =====================
        #region Constructor

        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly IGlobalSettingRepository _settingsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor; // if you need user info
        private readonly IErrorLogRepository _errorLogRepository;


        public GlobalSettingsController(
            IGlobalSettingRepository settingsRepository,
            IErrorLogRepository errorLogRepository,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _errorLogRepository = errorLogRepository;
            _settingsRepository = settingsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Global SEtting";
            base.OnActionExecuting(context);
        }
        #endregion

        //=================== Model Binding =====================
        #region Model_Binding
        #endregion

        //=================== partial Binding =====================
        #region Partial_Binding
        [HttpGet]
        public async Task<IActionResult> GetUserSettings()
        {
            // 1) Identify the user (assuming user must be logged in)
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // 2) Load each setting by key. 
            //    Alternatively, you could write a single stored procedure 
            //    that returns all settings for this user at once.
            var themeDto = await _settingsRepository.GetByTenantCompanyUserKeyAsync(TenantId, CompanyId, userId, "theme");
            var layoutDto = await _settingsRepository.GetByTenantCompanyUserKeyAsync(TenantId, CompanyId, userId, "layout");
            var sidebarDto = await _settingsRepository.GetByTenantCompanyUserKeyAsync(TenantId, CompanyId, userId, "sidebarNavOptions");
            var fluidDto = await _settingsRepository.GetByTenantCompanyUserKeyAsync(TenantId, CompanyId, userId, "builderFluidSwitch");

            // 3) Construct a simple response object. 
            //    Provide defaults if the DB returns null.
            var response = new
            {
                theme = themeDto?.SettingValue ?? "default",
                layout = layoutDto?.SettingValue ?? "default",
                sidebarNavOptions = sidebarDto?.SettingValue ?? "pills",
                fluid = fluidDto?.SettingValue ?? "false"
            };

            return Ok(response);
        }

        // POST /GlobalSettings/SetUserSetting
        [HttpPost]
        public async Task<IActionResult> SetUserSetting([FromBody] SetUserSettingRequest request)
        {
            // 1) Figure out TenantId, CompanyId, or UserId as needed
            // If you're using numeric user IDs, or string IDs, adjust accordingly.
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // 2) Check if a row already exists
            var existing = await _settingsRepository.GetByTenantCompanyUserKeyAsync(TenantId, CompanyId, userId, request.Key);

            if (existing == null)
            {
                // 3a) Insert
                var newSetting = new GlobalSettingsDto
                {
                    TenantId = TenantId,      // or parse from the request if you have multi-tenant
                    CompanyId = CompanyId,     // or parse from the request
                    UserId = userId,
                    SettingKey = request.Key,
                    SettingValue = request.Value,
                    ValueType = "string"
                };
                var newId = await _settingsRepository.AddAsync(newSetting);
            }
            else
            {
                // 3b) Update
                existing.SettingValue = request.Value;
                existing.UpdatedDate = DateTime.UtcNow;
                await _settingsRepository.UpdateAsync(existing);
            }

            return Ok(new { success = true });
        }
        public class SetUserSettingRequest
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
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
