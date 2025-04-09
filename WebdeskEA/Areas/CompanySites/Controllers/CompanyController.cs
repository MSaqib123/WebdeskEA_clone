using AutoMapper;
using Elfie.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Core.Extension;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace CRM.Areas.CompanySites.Controllers
{
    [Area("CompanySites")]
    [Authorize(Roles = "Tenant Admin,SuperAdmin,Admin,AuthUser")]
    public class CompanyController : Controller
    {
        //_______ Constructor _________
        #region Constructor
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);


        private readonly ICityRepository _cityRepository ;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateProvinceRepository _stateProvinceRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IImageService _imageService;

        public CompanyController(
            ICompanyRepository companyRepository, 
            IErrorLogRepository errorLogRepository,
            IStateProvinceRepository stateProvinceRepository,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IImageService imageService
            )
        {
            _stateProvinceRepository = stateProvinceRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _companyRepository = companyRepository;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _imageService = imageService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Company";
            ViewData["Title"] = "Company";
            base.OnActionExecuting(context);
        }

        #endregion

        //_______ Model Binding _________
        #region Model_Binding
        // GET: /Company/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpContext.Session.Remove("Source");
                if (TenantId > 0)
                {
                    var companies = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                    return View(companies);
                }
                else
                {
                    var companies = await _companyRepository.GetAllAsync();
                    return View(companies);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> CompanyByTenant(int Id)
        {
            try
            {
                HttpContext.Session.SetString("Source", "CompanyByTenant");
                var companies = await _companyRepository.GetAllByTenantIdAsync(Id);
                companies.Select(x => x.TenantId = Id).ToList();
                return View(companies);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }


        // GET: /Company/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                var source = TempData["Source"] as string;
                if (source == "CompanyTenant")
                {
                    return RedirectToAction(nameof(CompanyByTenant), new { Id = company.TenantId });
                }
                return RedirectToAction(nameof(Index)); // Default redirect to Index
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }

        }

        // GET: /Company/Create
        public async Task<IActionResult> Create(int TenantId = 0)
        {
            try
            {
                var dto = new CompanyDto();
                dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync();
                dto.CountryDtoList = await _countryRepository.GetAllAsync();
                dto.CityDtoList = await _cityRepository.GetAllListAsync();

                //for companyFilter
                dto.TenantId = TenantId;
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: /Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyDto dto, IFormFile Logo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dto.ParentCompanyId = CompanyId;
                    dto.TenantId = TenantId;

                    // ---------- Logo Creation Logic ----------
                    if (Logo != null)
                    {
                        dto.Logo = _imageService.UploadImage(Logo, "uploads/Company/Logo");
                    }
                    // ------------------------------------------

                    if (TenantId > 0)
                    {
                        var companies = await _companyRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                        if (companies.Count() < TenantCompanies)
                        {
                            var newCompanyId = await _companyRepository.AddAsync(dto);
                            TempData["Success"] = "Company Added Successfully";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["Error"] = "Upgrade The Package please";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        var newCompanyId = await _companyRepository.AddAsync(dto);
                        var source = HttpContext.Session.GetString("Source");
                        if (source == "CompanyByTenant")
                        {
                            TempData["Success"] = "Company Added Successfully";
                            return RedirectToAction(nameof(CompanyByTenant), new { Id = dto.TenantId });
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync();
                    dto.CountryDtoList = await _countryRepository.GetAllAsync();
                    dto.CityDtoList = await _cityRepository.GetAllListAsync();
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }


        // GET: /Company/Edit/5
        public async Task<IActionResult> Edit(int id,int TenantId = 0)
        {
            try
            {
                var dto = await _companyRepository.GetByIdAsync(id);
                if (dto == null)
                {
                    return NotFound();
                }
                dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync() ?? new List<StateProvinceDto>();
                dto.CountryDtoList = await _countryRepository.GetAllAsync() ?? new List<CountryDto>();
                dto.CityDtoList = await _cityRepository.GetAllListAsync() ?? new List<CityDto>();
                dto.TenantId = TenantId;
                if (dto == null)
                {
                    return NotFound();
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }

        // POST: /Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyDto dto, IFormFile Logo)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("Company ID mismatch");
                }

                // Populate dropdown lists or other related data
                dto.StateProvinceDtoList = await _stateProvinceRepository.GetAllListAsync();
                dto.CountryDtoList = await _countryRepository.GetAllAsync();
                dto.CityDtoList = await _cityRepository.GetAllListAsync();

                if (ModelState.IsValid)
                {
                    // Retrieve the existing company record to get the current logo
                    var company = await _companyRepository.GetByIdAsync(dto.Id);
                    if (company == null)
                    {
                        return NotFound();
                    }

                    // ---------- Image (Logo) Creation and Updation Logic ----------
                    if (Logo != null)
                    {
                        // Delete the existing logo image from storage (if any)
                        _imageService.DeleteImage(company.Logo, "uploads/Company/Logo");

                        // Upload the new logo image and set the dto.Logo property
                        dto.Logo = _imageService.UploadImage(Logo, "uploads/Company/Logo");
                    }
                    else
                    {
                        // Preserve the existing logo if no new file is uploaded
                        dto.Logo = company.Logo;
                    }
                    // ----------------------------------------------------------------

                    // Set additional properties (e.g., TenantId from session)
                    var tenantId = HttpContext.Session.GetInt32("TId");
                    var source = HttpContext.Session.GetString("Source");
                    if (tenantId.HasValue)
                    {
                        dto.TenantId = tenantId.Value;
                    }

                    // Update the company record in the repository
                    var result = await _companyRepository.UpdateAsync(dto);
                    TempData["Success"] = "Company Updated Successfully";

                    if (result == 0)
                    {
                        return NotFound();
                    }

                    if (source == "CompanyByTenant")
                    {
                        return RedirectToAction(nameof(CompanyByTenant), new { Id = dto.TenantId });
                    }
                    return RedirectToAction(nameof(Index)); // Default redirect to Index
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }


        // GET: /Company/Delete/5
        public async Task<IActionResult> Delete(int id,int TenantId = 0)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            company.TenantId = TenantId;
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: /Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int TenantId = 0)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                _imageService.DeleteImage(company.Logo, "uploads/Company/Logo");
                var result = await _companyRepository.DeleteAsync(id);
                if (result == 0)
                {
                    return NotFound();
                }

                var source = HttpContext.Session.GetString("Source");
                if (source == "CompanyByTenant")
                {
                    return RedirectToAction(nameof(CompanyByTenant), new { Id = TenantId });
                }
                return RedirectToAction(nameof(Index)); // Default redirect to Index
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Admin", statusCode = 500, errorMessage = ex.Message });
            }
        }


        // GET: /Company/Search?name={name}
        public async Task<IActionResult> Search(string name)
        {
            var companies = await _companyRepository.GetByNameAsync(name);
            return View("Index", companies);  // Reuse the Index view to display the search results
        }

        // GET: /Company/Paginated?pageIndex={pageIndex}&pageSize={pageSize}&filter={filter}
        public async Task<IActionResult> Paginated(int pageIndex, int pageSize, string filter)
        {
            var companies = await _companyRepository.GetPaginatedAsync(pageIndex, pageSize, filter);
            return View("Index", companies);  // Reuse the Index view to display paginated results
        }
        #endregion


        //_______ Error Log _________
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
