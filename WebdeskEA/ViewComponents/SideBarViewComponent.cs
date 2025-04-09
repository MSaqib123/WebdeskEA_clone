using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebdeskEA.ViewComponents
{
    [Authorize]
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IModuleRepository _ModuleRepo;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;

        public SideBarViewComponent(IModuleRepository ModuleRepo, IProjectPermissionRepository projectPermissionRepository = null)
        {
            _ModuleRepo = ModuleRepo;
            _ProjectPermissionRepository = projectPermissionRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return View(new ModuleDto());
            }
            var module = await _ProjectPermissionRepository.GetPermTenantUserSideBarByUserId(userId);

            ModuleDto vm = new ModuleDto
            {
                MenuList = module,
                ModuleList = await _ModuleRepo.GetAllAsync()
            };
            return View(vm);
        }
    }
}

