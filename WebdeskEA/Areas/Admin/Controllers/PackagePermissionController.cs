using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AllRolesPolicy")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    public class PackagePermissionController : Controller
    {
        #region Constractur
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackagePermissionRepository _packagePermissionRepository;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;

        public PackagePermissionController(
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IPackageTypeRepository packageTypeRepository,
            IPackageRepository packageRepository,
            IPackagePermissionRepository packagePermissionRepository)
        {
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageTypeRepository = packageTypeRepository;
            _packageRepository = packageRepository;
            _packagePermissionRepository = packagePermissionRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "PackagePermissions";
            base.OnActionExecuting(context);
        }
        #endregion

        #region Model_Binding
        // GET: Modules/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dto = await _packageRepository.GetAllAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        
        //- Detail 
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var Dto = await _packageTypeRepository.GetByIdAsync(id);
                return View(Dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                PackagePermissionDto dto = new PackagePermissionDto();
                var PackagePermission = await _packagePermissionRepository.GetPackagePermissionForUpdateBaseAsync();
                return View(PackagePermission);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackagePermissionDto dto)
        {
            try
            {
                var model = dto.ModuleIds.Select(mId => new PackagePermissionDto
                {
                    PackageId = dto.PackageId,
                    PackageName = dto.PackageName,
                    ModuleId = mId
                }).ToList();
                await _packagePermissionRepository.BulkAddPackagePermissionAsync(model,dto.packageDto);
                TempData["Success"] = "PackagePermisisons Added Successfully";
                return RedirectToAction(nameof(Index));
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
                var PackagePermission = await _packagePermissionRepository.GetPackagePermissionForUpdateBaseAsync(id);
                var Package = await _packageRepository.GetByIdAsync(id);
                PackagePermission.PackageName = Package?.PackageName ?? "Company Not Found";
                PackagePermission.PackageId = id;
                if (PackagePermission == null)
                {
                    return NotFound();
                }
                return View(PackagePermission);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Modules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PackagePermissionDto dto)
        {
            try
            {
                var model = dto.ModuleIds.Select(mId => new PackagePermissionDto
                {
                    PackageId = dto.PackageId,
                    PackageName = dto.PackageName,
                    ModuleId = mId
                }).ToList();
                await _packagePermissionRepository.BulkAddPackagePermissionAsync(model,dto.packageDto);
                TempData["Success"] = "PackagePermisisons Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET : Module/Delete/Reset/{id}
        [HttpGet]
        public async Task<IActionResult> ResetPackagePermission(int Id)
        {
            try
            {
                var module = await _packagePermissionRepository.GetAllByPackageIdAsync(Id);
                if (module.Count() > 0)
                {
                    await _packagePermissionRepository.DeleteByPackageIdAsync(Id);
                    TempData["Success"] = "PackagePermisisons Reset Successfully";
                }
                else
                {
                    ModelState.AddModelError("", "Failed to Reset PackagePermission.");
                    TempData["Error"] = "PackagePermisisons are Nulls";
                }
                return RedirectToAction(nameof(Index));
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
                var PackagePermission = await _packagePermissionRepository.GetPackagePermissionForUpdateBaseAsync(id);
                var Package = await _packageRepository.GetByIdAsync(id);
                PackagePermission.PackageName = Package?.PackageName ?? "Company Not Found";
                PackagePermission.PackageId = id;
                if (PackagePermission == null)
                {
                    return NotFound();
                }
                return View(PackagePermission);
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
                var module = await _packagePermissionRepository.GetAllByPackageIdAsync(id);
                if (module.Count() > 0)
                {
                    await _packagePermissionRepository.DeleteByPackageIdAsync(id);
                    await _packageRepository.DeleteAsync(id);
                    TempData["Success"] = "PackagePermisisons Deleted Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete PackagePermision.");
                    TempData["Error"] = "PackagePermisisons are Nulls";
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
