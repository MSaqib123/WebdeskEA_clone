using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;

namespace WebdeskEA.Core.Middlware
{
    public class UserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly ISubscriptionUpgrades_PermisssionsRepository _SubscriptionUpgrades_PermisssionsRepository;

        public UserSessionService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            ITenantRepository tenantRepository,
            IFinancialYearRepository financialYearRepository,
            ISubscriptionUpgrades_PermisssionsRepository SubscriptionUpgrades_PermisssionsRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _financialYearRepository = financialYearRepository;
            _SubscriptionUpgrades_PermisssionsRepository = SubscriptionUpgrades_PermisssionsRepository;
        }

        public async Task InitializeUserSessionIfNeededAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            // Check if user is logged in and session has not been initialized yet
            if (context.User.Identity.IsAuthenticated && !context.Session.Keys.Contains("TenantId"))
            {
                // Get the current user
                var user = await _userManager.GetUserAsync(context.User);
                // If the user is not a SuperAdmin, initialize session values
                if (user != null && user.UserName != "SuperAdmin@gmail.com")
                {
                    context.Session.SetInt32("TenantId", user.TenantId ?? 0);
                    context.Session.SetInt32("CompanyId", user.CompanyId ?? 0);
                    context.Session.SetString("isTenantAdmin", user.isTenantAdmin.ToString());

                    // Fetch tenant data if TenantId is available
                    if (user.TenantId.HasValue)
                    {
                        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId.Value);
                        context.Session.SetInt32("TotalCompaniesAllowed", tenant.TenantCompanies);
                        context.Session.SetInt32("TotalUsersAllowed", tenant.TenantUsers);
                    }

                    // Fetch Package data of CurrentUser
                    var Packages = await _SubscriptionUpgrades_PermisssionsRepository.GetByUserIdAsync(user.Id);
                    if (Packages.Count() > 0)
                    {
                        context.Session.SetInt32("PackageId", Packages.FirstOrDefault()!.PackageId);
                    }

                    // Fetch FY data of CurrentUser
                    var fy = await _financialYearRepository.GetAllByTenantCompanyIdAsync(user.TenantId??0,user.CompanyId??0);
                    if(fy.Any(x=>x.isLock == true))
                    {
                        context.Session.SetInt32("FinancialYearId", fy.Where(x=>x.isLock).FirstOrDefault()!.Id);
                    }
                }
            }
        }
    }

}
