using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebdeskEA.Domain.RepositoryDapper;

namespace WebdeskEA.ViewComponents
{
    [Authorize]
    public class TemplateBuilderViewComponent : ViewComponent
    {
        private readonly IModuleRepository _ModuleRepo;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;

        public TemplateBuilderViewComponent(IModuleRepository ModuleRepo, IProjectPermissionRepository projectPermissionRepository)
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
                return View(new ModuleDto()); // Returning an empty ModuleDto if no userId
            }

            List<ModuleDto> listmenu = new List<ModuleDto>();
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
            return View(vm);
        }
    }
}

