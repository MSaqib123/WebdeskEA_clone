using WebdeskEA.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WebdeskEA.Models.MappingSingleModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    //[Authorize(Roles = "Tenant Admin")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class DSB_HomeController : Controller
    {
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);

        private readonly ILogger<DSB_HomeController> _logger;
        private readonly IWebHostEnvironment _iWeb;
        private readonly IDashboardRepository _dashboardRepository;

        public DSB_HomeController(
            ILogger<DSB_HomeController> logger,
            IWebHostEnvironment iWeb,
            IDashboardRepository dashboardRepository)
        {
            _logger = logger;
            _iWeb = iWeb;
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IActionResult> Index()
        {
            DashboardDto  model = await _dashboardRepository.GetAllDashboardStatisticsAsync(TenantId, CompanyId, FinancialYearId);
            return View(model);
        }

        private bool HasPermission(int permissions, int permissionIndex)
        {
            return (permissions & (1 << permissionIndex)) != 0;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
