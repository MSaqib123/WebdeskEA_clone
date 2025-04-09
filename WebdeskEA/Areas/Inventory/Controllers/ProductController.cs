using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class ProductController : Controller
    {
        //_______ Error Log ______
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ICOARepository _COARepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        private readonly IImageService _imageService;

        public ProductController(
            ICategoryRepository categoryRepository,
            IImageService imageService,
            IProductCOARepository ProductCOARepository,
            ICOARepository COARepository,
            ITaxMasterRepository taxMasterRepository,
            IBrandRepository brandRepository,
            IProductRepository productRepository,
            IPackageRepository packageRepository,
            IPackageTypeRepository packageTypeRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IFinancialYearRepository financialYearRepository,
            ITenantRepository tenantRepository,
            ITenantPrefixService tenantPrefixService)
        {
            _categoryRepository = categoryRepository;
            _imageService = imageService;
            _productCOARepository = ProductCOARepository;
            _COARepository = COARepository;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
            _financialYearRepository = financialYearRepository;
            _tenantRepository = tenantRepository;
            _taxMasterRepository = taxMasterRepository;
            _tenantPrefixService = tenantPrefixService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Product / Service";
            base.OnActionExecuting(context);
        }

        #endregion

        //_______ Model Binding ______
        #region Model_Binding
        // GET: Modules/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<ProductDto> dto;
                if (TenantId > 0)
                {
                    dto = await _productRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                else
                {
                    dto = await _productRepository.GetAllAsync();
                }

                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }


        // If user navigates to /Admin/Product/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ProductDto dto = await BuildProductDtoForCreateAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                if (dto.ImageForSave != null)
                {
                    dto.Image = "/uploads/Product/" + _imageService.UploadImage(dto.ImageForSave, "uploads/Product");
                }
                dto.TenantId = TenantId;
                dto.CompanyId = CompanyId;
                var newId = await _productRepository.AddTransactionAsync(dto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(dto);
            }
        }

        // GET: Modules/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _productRepository.GetByIdAsync(id) ?? new ProductDto();
                dto.BrandDtos = await _brandRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COAIncomeDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "income");
                dto.COAExpenseDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "expenses");
                dto.SelectedIncomeCOAs = await _productCOARepository.GetSelectedSaleCOAsByProductIdAsync(id);
                dto.SelectedExpenseCOAs = await _productCOARepository.GetSelectedBuyCOAsByProductIdAsync(id);
                dto.CategoryDtos = await _categoryRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                if (dto == null)
                    return NotFound();

                return View(dto);
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
        public async Task<IActionResult> Edit(ProductDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = await _productRepository.GetByIdAsync(dto.Id);
                    if (model == null)
                        return NotFound();
                    if (model != null)
                    {
                        _imageService.DeleteImage(model.Image!, "uploads/Product");
                        dto.Image = "/uploads/Product/" + _imageService.UploadImage(dto.ImageForSave, "uploads/Product");
                    }
                    else
                    {
                        dto.Image = model.Image;
                    }
                    dto.TenantId = TenantId;
                    dto.CompanyId = CompanyId;
                    var id = await _productRepository.UpdateTransactionAsync(dto);
                    if (id > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update module.");
                    }
                }
                dto.BrandDtos = await _brandRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.COAIncomeDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "income");
                dto.COAExpenseDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "expenses");
                dto.SelectedIncomeCOAs = await _productCOARepository.GetSelectedSaleCOAsByProductIdAsync(dto.Id);
                dto.SelectedExpenseCOAs = await _productCOARepository.GetSelectedBuyCOAsByProductIdAsync(dto.Id);
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
                var module = await _productRepository.GetByIdAsync(id);
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
                var model = await _productRepository.GetByIdAsync(id);
                if (model == null)
                    return NotFound();

                if (model == null)
                    return NotFound();
                if (model != null)
                {
                    _imageService.DeleteImage(model.Image, "uploads/Category");
                }


                var result = await _productRepository.DeleteAsync(id);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to delete module.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion


        //_______ Helper Method ______
        #region Helper_Method
        // ================== Helper to build the ProductDto ==================
        private async Task<ProductDto> BuildProductDtoForCreateAsync()
        {
            var dto = new ProductDto();
            dto.ProductCode = await _tenantPrefixService.InitializeGenerateCode(
                TenantId, CompanyId, "", PrefixType.Product, true);
            dto.TaxMasterDtos = await _taxMasterRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            dto.BrandDtos = await _brandRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            dto.COAIncomeDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "income");
            dto.COAExpenseDtos = await _COARepository.GetAllByCompanyIdOrAccountTypeAsync(CompanyId, "expenses");
            dto.CategoryDtos = await _categoryRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
            return dto;
        }
        #endregion


        //_______ Partial Binding _________
        #region Partial_Binding

        //============== Product Openig Modal ==============
        #region Product_Opening_Modal
        [HttpGet]
        public async Task<IActionResult> CreatePartial(string typeOfPartialView = "Purchase") //Purchase ?? Sale
        {
            try
            {
                ProductDto dto = await BuildProductDtoForCreateAsync();
                dto.typeOfPartialView = typeOfPartialView;
                return PartialView("_CreateProductPartial", dto);
            }
            catch (Exception ex)
            {
                // If an error occurs loading data for the partial,
                // you could return a partial with an error message or a 500.
                return StatusCode(500, "Failed to load product form: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePartialPost(ProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateProductPartial", model);
            }

            try
            {
                if (model.ImageForSave != null)
                {
                    model.Image = "/uploads/Product/" + _imageService.UploadImage(model.ImageForSave, "uploads/Product");
                }

                // Save the product
                model.TenantId = TenantId;
                model.CompanyId = CompanyId;
                var newId = await _productRepository.AddTransactionAsync(model);

                // Return JSON so the client can close the modal and refresh product dropdown
                return Json(new
                {
                    success = true,
                    newProductId = newId,
                    newProductName = model.ProductName,
                    newProductUnitPrice = model.ProductPrice
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
                return PartialView("_CreateProductPartial", model);
            }
        }
        #endregion


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
