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
    public class UserRightManagerController : Controller
    {
        //_______ Constructor ______
        #region Constractur
        private readonly IApplicationUserRepository _userRepo;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRightManagerController(
            IApplicationUserRepository userRepo,
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
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "User Rights Manager";
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
                var users = await _userRepo.GetAllUsersAsync();
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
        public async Task<IActionResult> Create(string id)
        {
            try
            {
                UserRightDto dto = new UserRightDto();
                dto.UserRigthsList = await _userRightRepo.GetAllListByUserIdAsync(id);
                var userDetail = await _userRepo.GetByIdAsync(id);
                dto.UserName = userDetail.UserName;
                return View(dto);
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
        public async Task<IActionResult> Create(UserRightDto model)
        {
            try
            {
                //var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "GuestUser";
                //model.UserRigthsList = model.UserRigthsList.Select(dto =>
                //{
                //    dto.CreatedBy = userId;
                //    dto.ModifiedBy = userId
                //});
                var id = await _userRightRepo.BulkAddAsync(model.UserRigthsList);
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
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                UserRightDto dto = new UserRightDto();
                dto.UserRigthsList = await _userRightRepo.GetAllListByUserIdAsync(id);
                var userDetail = await _userRepo.GetByIdAsync(id);
                dto.UserName = userDetail.UserName;
                dto.UserId = userDetail.Id;
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: UserRights/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var result = await _userRightRepo.DeletesAllRightsOfUserAsync(id);
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
