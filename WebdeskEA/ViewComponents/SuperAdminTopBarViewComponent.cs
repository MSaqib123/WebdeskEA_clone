using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebdeskEA.Domain.Service;
using Microsoft.AspNetCore.Identity;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.ViewModels;

namespace WebdeskEA.ViewComponents
{
    [Authorize]
    public class SuperAdminTopBarViewComponent : ViewComponent
    {
        private readonly IModuleRepository _ModuleRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;
        public SuperAdminTopBarViewComponent(IModuleRepository ModuleRepo, UserManager<ApplicationUser> userManager, IImageService imageService, IProjectPermissionRepository projectPermissionRepository)
        {
            _ModuleRepo = ModuleRepo;
            _imageService = imageService;
            _userManager = userManager;
            _ProjectPermissionRepository = projectPermissionRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;

            //================ Dyanamic SideBar =================
            #region SideBar ____ User

            #region UserNotFound
            if (userId == null && userName == null)
            {
                return View(new HeaderViewModel
                {
                    ProfileImageUrl = "~/uploads/ProjectIcon/undefinedIcon.webp", // Use a fixed image for SuperAdmin
                    UserName = "Guest",
                    Name = "Guest User",
                    moduleDto = new ModuleDto() // Return an empty ModuleDto
                });
            }
            #endregion

            List<ModuleDto> listmenu = new List<ModuleDto>();

            #region SuperAdmin
            if (userName == "SuperAdmin@gmail.com")
            {
                // Hardcoded modules for SuperAdmin (you can customize these)
                listmenu = new List<ModuleDto>
                {
                    new ModuleDto { ModuleName = "Dashboard", ModuleUrl = "/admin/dashboard", ModuleIcon = "fas fa-tachometer-alt", ParentModuleId = 0 },
                    new ModuleDto { ModuleName = "User Management", ModuleUrl = "/admin/users", ModuleIcon = "fas fa-users", ParentModuleId = 0 }
                };

                var vmSuperAdmin = new ModuleDto
                {
                    MenuList = listmenu,
                    ModuleList = listmenu // Return the same list for now, adjust as needed
                };

                var superAdminDetail = new HeaderViewModel
                {
                    ProfileImageUrl = "~/uploads/ProjectIcon/superAdmin.png", // Use a fixed image for SuperAdmin
                    UserName = "SuperAdmin",
                    Name = "Super Admin",
                    moduleDto = vmSuperAdmin
                };

                return View(superAdminDetail);
            }
            #endregion

            var module = await _ProjectPermissionRepository.GetPermTenantUserSideBarByUserId(userId);
            foreach (var item in module)
            {
                ModuleDto m = new ModuleDto
                {
                    ModuleName = item.ModuleName,
                    ModuleUrl = item.ModuleUrl,
                    ModuleIcon = item.ModuleIcon,
                    ParentModuleId = item.ParentModuleId
                };
                listmenu.Add(m);
            }
            ModuleDto vm = new ModuleDto
            {
                MenuList = listmenu,
                ModuleList = await _ModuleRepo.GetAllAsync()
            };

            //================ User Detail for TopBarNavbar =================
            #region Userdetail
            var user = await _userManager.FindByNameAsync(userName);
            var profileImageUrl = _imageService.GetImagePath(user.ProfileImage, "~/uploads/users/") ?? "~/uploads/ProjectIcon/undefinedIcon.webp";//user != null ? _imageService.GetImagePath(user.ProfileImage, "~/uploads/users/") : "~/Template/img/profiles/default.jpg";

            var userDetail = new HeaderViewModel
            {
                ProfileImageUrl = profileImageUrl,
                UserName = user?.UserName ?? "Unknown",
                Name = user?.Name ?? "Unknown",
                moduleDto = vm
            };
            #endregion

            #endregion

            return View(userDetail);
        }
    }
}

