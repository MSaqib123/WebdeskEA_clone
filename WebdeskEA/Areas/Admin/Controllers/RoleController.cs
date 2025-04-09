using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Build.Framework;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class RoleController : Controller
    {
        //_______ Constructor ______
        #region Constractur
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleController(
            IApplicationUserRepository userRepo,
            IRolePermissionRepository rolePermissionRepository,
            IRoleRepository roleRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _httpContextAccessor = httpContextAccessor;
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Roles";
            base.OnActionExecuting(context);
        }
        #endregion

        //_______ Model Binding ______
        #region Model_Binding
        // GET: Modules/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    var id = await _roleRepository.AddAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction("Index", "RolePermissions", new { area = "Admin" });
                    }
                    else
                    {
                        return View(dto);
                    }
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if(id == 0)
                    return NotFound();
                var dto = await _roleRepository.GetByIdAsync(id);
                if (dto == null)
                    return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = await _roleRepository.UpdateAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction("Index", "RolePermissions", new { area = "Admin" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);

                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var module = await _roleRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                return View(module);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var module = await _roleRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                var result = await _roleRepository.DeleteAsync(id);
                if (result > 0)
                {
                    return RedirectToAction("Index", "RolePermissions", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete module.");
                    return View(module);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion

        //_______ Error Log ______
        #region LogError
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
