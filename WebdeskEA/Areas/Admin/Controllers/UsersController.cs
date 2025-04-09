using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace CRM.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "SuperAdmin,Admin,AuthUser")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class UsersController : Controller
    {
        //_______ Constructor ______
        #region Constructor
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        private readonly IApplicationUserRepository _userRepo;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackagePermissionRepository _packagePermissionRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantPermissionRepository _TenantPermissionRepository;

        public UsersController(
            IApplicationUserRepository userRepo,
            IRoleRepository roleRepository,
            IMapper mapper,
            IImageService imageService,
            IErrorLogRepository errorLogRepository,
            IPackageRepository packageRepository,
            IPackagePermissionRepository packagePermissionRepository,
            ITenantRepository tenantRepository,
            ITenantPermissionRepository TenantPermissionRepository
            )
        {
            _userRepo = userRepo;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _imageService = imageService;
            _packageRepository = packageRepository;
            _packagePermissionRepository = packagePermissionRepository;
            _tenantRepository = tenantRepository;
            _TenantPermissionRepository = TenantPermissionRepository;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "User";
            base.OnActionExecuting(context);
        }

        #endregion

        //_______ Model Binding ______
        #region Module_Binding

        // GET: Users/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (TenantId > 0)
                {
                    var dto = await _userRepo.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    return View(dto);
                }
                else
                {
                    var dto = await _userRepo.GetAllUsersAsync();
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex, "Index");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Users/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ApplicationUserDto dto = new ApplicationUserDto();
            dto.RoleDtos = await _roleRepository.GetAllAsync();
            return View(dto);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserDto model, IFormFile profileImage)
        {
            try
            {
                ModelState.Remove("Id");
                //ModelState.Remove("ProfileImage");
                if (ModelState.IsValid)
                {
                    if (TenantId > 0)
                    {
                        if (profileImage != null)
                        {
                            model.ProfileImage = _imageService.UploadImage(profileImage, "uploads/users");
                        }

                        var dto = await _userRepo.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        if (dto.Count() < TenantUsers)
                        {
                            //----- 1 Add User ----
                            model.TenantId = TenantId;
                            model.CompanyId = CompanyId;
                            var result = await _userRepo.AddAsync(model);

                            bool inserted = await InsertTenantPermission("Add",model);

                            if(inserted)
                            {
                                TempData["Success"] = "User Added Successfully";
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                TempData["Error"] = "Permission Faild";
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Upgrade The Package please";
                            return RedirectToAction(nameof(Index));
                        }

                    }
                    else
                    {
                        if (profileImage != null)
                        {
                            model.ProfileImage = _imageService.UploadImage(profileImage, "uploads/users");
                        }

                        var result = await _userRepo.AddAsync(model);

                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    
                }
                model.RoleDtos = await _roleRepository.GetAllAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                await LogError(ex, "Create");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // GET: Users/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var dto = await _userRepo.GetByIdAsync(id);
                dto.RoleDtos = await _roleRepository.GetAllAsync();
                if (dto == null)
                    return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex, "Edit");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUserDto model, IFormFile profileImage)
        {
            try
            {
                ModelState.Remove("ProfileImage");
                if (ModelState.IsValid)
                {
                    var user = await _userRepo.GetByIdAsync(model.Id);
                    if (user == null)
                        return NotFound();

                    // Preserve existing image if no new image is uploaded
                    if (profileImage != null)
                    {
                        _imageService.DeleteImage(user.ProfileImage!, "uploads/users");
                        model.ProfileImage = _imageService.UploadImage(profileImage, "uploads/users");
                    }
                    else
                    {
                        // Preserve the existing image
                        model.ProfileImage = user.ProfileImage;
                    }
                    //___________ 1. Update User _____
                    //___________ 2. Update Role _______
                    model.TenantId = TenantId;
                    model.CompanyId = CompanyId;
                    var result = await _userRepo.UpdateAsync(model);

                    if (result.Succeeded)
                    {
                        bool inserted = await InsertTenantPermission("Edit",model);
                        if (inserted)
                        {
                            TempData["Success"] = "User Updated Successfully";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag["Error"] = "Permission Faild";
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                model.RoleDtos = await _roleRepository.GetAllAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                await LogError(ex, "Edit");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        // GET: Users/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                await LogError(ex, "Delete");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: Users/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(id);
                if (user == null)
                    return NotFound();

                _imageService.DeleteImage(user.ProfileImage, "uploads/users");
                var result = await _userRepo.DeleteAsync(id);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex, "DeleteConfirmed");
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        #endregion


        //_______ Helper Method _____
        #region HelperMethod
        private async Task<bool> InsertTenantPermission(string action , ApplicationUserDto model)
        {
            try
            {
                //___________ 3. Update Permissions _______
                var package = await _packageRepository.GetByIdAsync(PackageId);
                var PackagePermission = await _packagePermissionRepository.GetAllByPackageIdAsync(package.Id);

                //========== Insert,Update Tenant Dapper =========
                var tenantId = 0;
                if (action == "Add")
                {
                    TenantDto tenantdto = new TenantDto();
                    tenantdto.TenantName = model.Name! ?? "User";
                    tenantdto.TenantCompanies = package.TotalCompany ?? 1; // Package Company
                    tenantdto.TenantUsers = package.TotalUser ?? 1; // Package User
                    tenantdto.TenantExpiryDate = DateTime.Now.AddYears(2);
                    tenantdto.TenantTypeId = 1;
                    tenantdto.TenantEmail = model.Email;
                    tenantId = await _tenantRepository.AddAsync(tenantdto);
                }
                if (action == "Edit")
                {
                    TenantDto tenantdto = await _tenantRepository.GetByIdAsync(TenantId);
                    tenantdto.TenantName = model.Name! ?? "User";
                    tenantdto.TenantCompanies = package.TotalCompany ?? 1; // Package Company
                    tenantdto.TenantUsers = package.TotalUser ?? 1; // Package User
                    tenantdto.TenantExpiryDate = DateTime.Now.AddYears(2);
                    tenantdto.TenantTypeId = 1;
                    tenantId = await _tenantRepository.UpdateAsync(tenantdto);
                }
                
                //========== Insert TenantPermission Dapper =========
                if (action == "Edit")
                    await _TenantPermissionRepository.DeleteByTenantIdAsync(tenantId);

                foreach (var item in PackagePermission)
                {
                    TenantPermissionDto tp = new TenantPermissionDto
                    {
                        TenantId = tenantId,
                        ModuleId = item.ModuleId ?? 1,
                        IsModuleActive = true
                    };
                    await _TenantPermissionRepository.AddAsync(tp);
                }

                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }
        #endregion

        //_______ Error Log ______
        #region ErrorHandling
        private async Task LogError(Exception ex, string action)
        {
            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);
            await _errorLogRepository.AddErrorLogAsync(
                area: "Company",
                controller: "CompanyUserController",
                actionName: action,
                formName: $"{action} Form",
                errorShortDescription: ex.Message,
                errorLongDescription: ErrorUtility.GetFullExceptionMessage(ex),
                statusCode: statusCode.ToString(),
                username: User.Identity?.Name ?? "GuestUser"
            );
        }

        #endregion
    }

}
