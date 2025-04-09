using WebdeskEA.Domain.Service;
using WebdeskEA.Models.ExternalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.ViewModels;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using System.Security.Claims;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.ViewComponents
{
    [Authorize]
    public class HeaderViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyRepository _companyRepository;
        private readonly IImageService _imageService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);


        public HeaderViewComponent(
            UserManager<ApplicationUser> userManager, 
            IImageService imageService, 
            IApplicationUserRepository applicationUserRepository, 
            ICompanyRepository companyRepository,
            IFinancialYearRepository financialYearRepository)
        {
            _userManager = userManager;
            _imageService = imageService;
            _applicationUserRepository = applicationUserRepository;
            _companyRepository = companyRepository;
            _financialYearRepository = financialYearRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = base.User.Identity.Name;

            if (userId != null)
            {
                //================ SuperAdmin =====================
                #region SuperAdmin
                if (userId == "SuperAdmin@gmail.com")
                {
                    // Return a view with hardcoded SuperAdmin details
                    return View(new HeaderViewModel
                    {
                        ProfileImageUrl = "~/uploads/ProjectIcon/superAdmin.png", // Use a fixed image for SuperAdmin
                        UserName = "SuperAdmin",
                        Name = "Super Admin" // Use any name you'd like for the SuperAdmin
                    });
                }
                #endregion

                //================ Regular User =====================
                #region Regular_user
                ApplicationUser user = await _userManager.FindByNameAsync(userId);
                string profileImageUrl = ((user != null) ? _imageService.GetImagePath(user.ProfileImage, "~/uploads/users/") : "~/Template/img/profiles/default.jpg");
                var compList = _companyRepository.GetAllAsync().Result.ToList().Where(x => x.TenantId == Convert.ToInt32(TenantId));
                var FinancialYearList = await _financialYearRepository.GetAllByCompanyId(CompanyId);
                return View(new HeaderViewModel
                {
                    ProfileImageUrl = profileImageUrl ?? "~/Template/img/160x160/img1.jpg",
                    UserName = user?.UserName ?? "Unknown",
                    Name = user?.Name ?? "Unknown",
                    CompanyDtos = compList,
                    FinancialYearDtos = FinancialYearList
                });

                #endregion
            }

            // In case the user is not logged in
            return View(new HeaderViewModel
            {
                ProfileImageUrl = "~/Template/img/160x160/img1.jpg"
            });
        }
    }
}
