using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class TenantController : Controller
    {
        #region Constractur
        private readonly ITenantRepository _TenantRepository;
        private readonly ITenantTypeRepository _TenantTypeRepository;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;

        public TenantController(
            ITenantRepository TenantRepository,
            ITenantTypeRepository TenantTypeRepository,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository)
        {
            _TenantRepository = TenantRepository;
            _TenantTypeRepository = TenantTypeRepository;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Tenant";
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
                var modules = await _TenantRepository.GetAllAsync();
                return View(modules);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Modules/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                TenantDto model = new TenantDto();
                model.TenantTypeDtoList = await _TenantTypeRepository.GetAllAsync();
                return View(model);
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
        public async Task<IActionResult> Create(TenantDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = await _TenantRepository.AddAsync(model);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View(model);
                    }
                }
                return View(model);
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
                var module = await _TenantRepository.GetByIdAsync(id);
                module.TenantTypeDtoList = await _TenantTypeRepository.GetAllAsync();
                if (module == null)
                    return NotFound();

                return View(module);
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
        public async Task<IActionResult> Edit(TenantDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = await _TenantRepository.UpdateAsync(model);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                return View(model);
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
                var module = await _TenantRepository.GetByIdAsync(id);
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
                var module = await _TenantRepository.GetByIdAsync(id);
                if (module == null)
                    return NotFound();

                var result = await _TenantRepository.DeletesAsync(id);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
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
