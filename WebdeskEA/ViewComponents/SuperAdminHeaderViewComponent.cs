using WebdeskEA.Domain.Service;
using WebdeskEA.Models.ExternalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.ViewModels;

namespace WebdeskEA.ViewComponents
{
    [Authorize]
    public class SuperAdminHeaderViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public SuperAdminHeaderViewComponent(UserManager<ApplicationUser> userManager, IImageService imageService, IApplicationUserRepository applicationUserRepository)
        {
            _userManager = userManager;
            _imageService = imageService;
            _applicationUserRepository = applicationUserRepository;
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

                return View(new HeaderViewModel
                {
                    ProfileImageUrl = profileImageUrl ?? "~/uploads/ProjectIcon/undefinedIcon.webp",
                    UserName = user?.UserName ?? "Unknown",
                    Name = user?.Name ?? "Unknown"
                });
                #endregion
            }

            // In case the user is not logged in
            return View(new HeaderViewModel
            {
                ProfileImageUrl = "~/Template/img/profiles/default.jpg"
            });
        }
    }
}
