using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
    public class RolePermissionsController : Controller
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

        public RolePermissionsController(
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
            ViewBag.NameOfForm = "Role Permission";
            base.OnActionExecuting(context);
        }
        #endregion

        //_______ Model Binding ______
        #region Model_Binding
        // GET: UserRights/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _roleRepository.GetAllAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: UserRights/Create
        [HttpGet]
        public async Task<IActionResult> Create(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return NotFound();
                }
                else
                {
                    RolePermissionDto dto = new RolePermissionDto();
                    dto.rolePermission = await _rolePermissionRepository.GetAllByRoleIdAsync(Id);
                    dto.roleDto = await _roleRepository.GetByIdAsync(Id);
                    dto.moduleDtos = await _moduleRepo.GetAllAsync();
                    dto.RoleId = Id;
                    return View(dto);
                }
                
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: UserRights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermissionDto model)
        {
            try
            {
                var id = await _rolePermissionRepository.BulkAddAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: UserRights/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int result = await _rolePermissionRepository.DeleteByRoleIdAsync(id);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete user right.");
                    return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = "Failed to delete user right." });
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
