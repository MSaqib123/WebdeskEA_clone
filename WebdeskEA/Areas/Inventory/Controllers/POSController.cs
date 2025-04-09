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
using WebdeskEA.Models.ViewModel;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;

namespace WebdeskEA.Areas.Admin.Controllers
{
    [Area("Inventory")]
    [Authorize(Policy = "AllRolesPolicy")]
    public class POSController : Controller
    {
        #region Constractur
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        protected int PackageId => Convert.ToInt32(MySessionExtensions.PackageId(HttpContext) ?? 0);
        protected int TenantCompanies => Convert.ToInt32(MySessionExtensions.TotalTenantCompanies(HttpContext) ?? 0);
        protected int TenantUsers => Convert.ToInt32(MySessionExtensions.TotalTenantUsers(HttpContext) ?? 0);
        protected int FinancialYearId => Convert.ToInt32(MySessionExtensions.FinancialYearId(HttpContext) ?? 0);
        private readonly IPOSConfigRepository _POSConfigRepository;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly ISIRepository _SIRepository;
        private readonly ISORepository _SORepository;
        private readonly ISODtlRepository _SODtlRepository;
        private readonly ISODtlTaxRepository _SODtlTaxRepository;
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IProductRepository _ProductRepository;
        private readonly IPIRepository _PIRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOSDtlRepository _OSDtlRepository;
        private readonly IOSRepository _OSRepository;
        private readonly IProductCOARepository _productCOARepository;
        private readonly ICOARepository _COARepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageTypeRepository _packageTypeRepository;
        private readonly IModuleRepository _moduleRepo;
        private readonly IUserRightsRepository _userRightRepo;
        private readonly IMapper _mapper;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ITenantPrefixService _tenantPrefixService;
        public POSController(
            ISODtlRepository SODtlRepository,
            ISODtlTaxRepository SODtlTaxRepository,
            IPOSConfigRepository POSConfigRepository,
            ISIRepository SIRepository,
            ISORepository SORepository,
            ICustomerRepository CustomerRepository,
            ICategoryRepository CategoryRepository,
            IProductRepository productRepository,
            ISupplierRepository SupplierRepository,
            IProductCOARepository ProductCOARepository,
            ICOARepository COARepository,
            ITaxMasterRepository taxMasterRepository,
            IBrandRepository brandRepository,
            IPackageRepository packageRepository,
            IPackageTypeRepository packageTypeRepository,
            IModuleRepository moduleRepo,
            IUserRightsRepository userRightRepo,
            IMapper mapper,
            IErrorLogRepository errorLogRepository,
            IFinancialYearRepository financialYearRepository,
            ITenantRepository tenantRepository,
            IOSDtlRepository sODtlRepository,
            IOSRepository sORepository,
            ITenantPrefixService tenantPrefixService,
            IPIRepository PIRepository)
        {
            _SODtlTaxRepository = SODtlTaxRepository;
            _SODtlRepository = SODtlRepository;
            _POSConfigRepository = POSConfigRepository;
            _SORepository = SORepository;
            _SIRepository = SIRepository;
            _CustomerRepository = CustomerRepository;
            _ProductRepository = productRepository;
            _CategoryRepository = CategoryRepository;
            _supplierRepository = SupplierRepository;
            _productCOARepository = ProductCOARepository;
            _COARepository = COARepository;
            _brandRepository = brandRepository;
            _moduleRepo = moduleRepo;
            _userRightRepo = userRightRepo;
            _mapper = mapper;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _packageRepository = packageRepository;
            _packageTypeRepository = packageTypeRepository;
            _financialYearRepository = financialYearRepository;
            _tenantRepository = tenantRepository;
            _taxMasterRepository = taxMasterRepository;
            _OSDtlRepository = sODtlRepository;
            _OSRepository = sORepository;
            _tenantPrefixService = tenantPrefixService;
            _PIRepository = PIRepository;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Opening Stock";
            base.OnActionExecuting(context);
        }
        #endregion
        
        
        #region Model_Binding
        // GET: Modules/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                PosVM dto = new PosVM();
                dto.CustomerDtos = await _CustomerRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                dto.POSConfig = await _POSConfigRepository.GetByActiveTenantIdCompanyIdAsync(TenantId,CompanyId);
                return View(dto);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return RedirectToAction("Error", "Error", new { area = "Settings", statusCode = 500, errorMessage = ex.Message });
            }
        }
        #endregion

        #region Partial_Binding
        [HttpGet]
        public async Task<IActionResult> GetProducts(string Category = "")
        {
            try
            {
                IEnumerable<ProductDto> dto = null;
                if (TenantId > 0)
                {
                    dto = await _ProductRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }

                if (dto == null)
                    return Json(new { success = false, message = "Data not found." });

                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var defaultImagePath = "/Template/img/400x400/img2.jpg";
                foreach (var product in dto)
                {
                    if (string.IsNullOrEmpty(product.Image))
                    {
                        product.Image = defaultImagePath;
                    }
                    else
                    {
                        // Convert the relative path into physical path to check existence
                        var fullPath = Path.Combine(webRootPath, product.Image.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                        if (!System.IO.File.Exists(fullPath))
                        {
                            product.Image = defaultImagePath;
                        }
                    }
                }

                return Json(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                IEnumerable<CategoryDto> dto = null;
                if (TenantId > 0)
                {
                    dto = await _CategoryRepository.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                }
                if (dto == null)
                    return Json(new { success = false, message = "Data not found." }); // Return failure response for AJAX

                // Return the JSON response
                return Json(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message }); // Error response in JSON
            }

        }



        // ----------------------------------------------------------------
        // 1) CompleteSale => Insert into SIDto + SIDtl
        // ----------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CompleteSale([FromBody] PosCartVM data)
        {
            try
            {
                if (data.CartItems == null || data.CartItems.Count == 0)
                {
                    return Json(new { success = false, message = "Cart is empty." });
                }

                // 1. Build a new Sales Invoice (SIDto)
                var si = new SIDto
                {
                    SICode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SI, true),
                    SIDate = DateTime.Now,
                    CustomerId = data.CustomerId,
                    SISubTotal = data.SubTotal,
                    SIDiscount = data.DiscountAmount,
                    SITotal = data.Total,
                    SITotalAfterVAT = data.Total,
                    TenantId = TenantId,
                    CompanyId = CompanyId,
                    FinancialYearId = FinancialYearId,
                    isPOS = true
                };

                // 2. Build detail lines
                var siDetails = new List<SIDtlDto>();
                foreach (var line in data.CartItems)
                {
                    var sidtl = new SIDtlDto
                    {
                        SIId = 0,  // EF sets later
                        ProductId = line.Id,
                        SIDtlQty = line.Qty,
                        SIDtlPrice = line.ProductPrice,
                        SIDtlTotal = line.Qty * line.ProductPrice
                    };
                    siDetails.Add(sidtl);
                }
                si.SIDtlDtos = siDetails;

                // 3. Build Tax lines
                var SIDtlTax = new List<SIDtlTaxDto>();
                foreach (var cartLine in data.CartTaxItems)
                {
                    var siTax = new SIDtlTaxDto
                    {
                        SIId = si.Id,
                        TaxAmount = cartLine.TaxAmount,
                        AfterTaxAmount = cartLine.AfterTaxAmount,
                        TaxId = 0 // or your real tax ID
                    };
                    SIDtlTax.Add(siTax);
                }
                si.SIDtlTaxDtos = SIDtlTax;

                //// 4. If we are converting from a hold, remove or update the hold
                //if (data.SoId.HasValue)
                //{
                //    //var so = _db.SODto
                //    //            .Include(x => x.SODtlDtos)
                //    //            .FirstOrDefault(x => x.Id == data.SoId.Value && x.isholdPOS == true);
                //    //if (so != null)
                //    //{
                //    //    // Option A: Delete it from the DB
                //    //    // This also means removing its detail lines (if cascade delete is not set up, do so or remove lines manually)
                //    //    //_db.SODto.Remove(so);

                //    //    // Option B (alternative): just un-hold it
                //    //    // so.isholdPOS = false;
                //    //    // _db.SODto.Update(so);
                //    //}
                //}

                // 5. Save changes
                int siid =await _SIRepository.AddTransactionAsync(si);

                int newSiId = siid;

                return Json(new { success = true, message = "Sale completed!", newId = newSiId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ----------------------------------------------------------------
        // 2) HoldSale => Insert into SODto + SODtl
        // ----------------------------------------------------------------
        //[HttpPost]
        //public async Task<IActionResult> HoldSale([FromBody] PosCartVM data)
        //{
        //    try
        //    {
        //        if (data.CartItems == null || data.CartItems.Count == 0)
        //        {
        //            return Json(new { success = false, message = "Cart is empty." });
        //        }

        //        // 1. Build new SODto
        //        var so = new SODto
        //        {
        //            SOCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SO, true),
        //            SODate = DateTime.Now,
        //            CustomerId = data.CustomerId,
        //            SOSubTotal = data.SubTotal,
        //            SODiscount = data.DiscountAmount,
        //            SOTotal = data.Total,
        //            SOTotalAfterVAT = data.Total,
        //            TenantId = TenantId,
        //            CompanyId = CompanyId,
        //            FinancialYearId = 1,
        //            isholdPOS = true  // Mark as "on hold from POS"
        //        };

        //        // 2. Build child lines
        //        var soDetails = new List<SODtlDto>();
        //        foreach (var line in data.CartItems)
        //        {
        //            var sodtl = new SODtlDto
        //            {
        //                SOId = so.Id, // EF sets later
        //                ProductId = line.Id,
        //                SODtlQty = line.Qty,
        //                SODtlPrice = line.ProductPrice,
        //                SODtlTotal = line.Qty * line.ProductPrice
        //            };
        //            soDetails.Add(sodtl);
        //        }
        //        so.SODtlDtos = soDetails;

        //        // 3. set Tax Against Each Sodtl
        //        var SIDtlTax = new List<SODtlTaxDto>();
        //        foreach (var cartLine in data.CartTaxItems)
        //        {
        //            var soTax = new SODtlTaxDto
        //            {
        //                SOId = so.Id,
        //                TaxAmount = cartLine.TaxAmount,
        //                AfterTaxAmount = cartLine.AfterTaxAmount,
        //                TaxId = 0 // or your real tax ID
        //            };
        //            SIDtlTax.Add(soTax);
        //        }
        //        so.SODtlTaxDtos = SIDtlTax;


        //        // 4. Insert into DB
        //        await _SORepository.AddTransactionAsync(so);

        //        return Json(new { success = true, message = "Order placed on hold.", newId = so.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> HoldSale([FromBody] PosCartVM data)
        {
            try
            {
                if (data.CartItems == null || data.CartItems.Count == 0)
                {
                    return Json(new { success = false, message = "Cart is empty." });
                }

                // If no SoId => create new, else => update existing
                if (data.SoId.HasValue && data.SoId.Value > 0)
                {
                    // -------------- UPDATE EXISTING HOLD --------------
                    var existingSo = await _SORepository.GetByIdAsync(data.SoId.Value);
                    if (existingSo == null)
                    {
                        return Json(new { success = false, message = "Cannot find existing hold with that SoId." });
                    }

                    // Overwrite fields
                    existingSo.CustomerId = data.CustomerId;
                    existingSo.SOSubTotal = data.SubTotal;
                    existingSo.SODiscount = data.DiscountAmount;
                    existingSo.SOTotal = data.Total;
                    existingSo.SOTotalAfterVAT = data.Total;
                    existingSo.isholdPOS = true; // still on hold
                    existingSo.SODate = DateTime.Now;

                    var soDetails = new List<SODtlDto>();
                    foreach (var line in data.CartItems)
                    {
                        soDetails.Add(new SODtlDto
                        {
                            SOId = existingSo.Id,
                            ProductId = line.Id,
                            SODtlQty = line.Qty,
                            SODtlPrice = line.ProductPrice,
                            SODtlTotal = line.Qty * line.ProductPrice
                        });
                    }
                    existingSo.SODtlDtos = soDetails;

                    // Similarly for SODtlTax
                    var SIDtlTax = new List<SODtlTaxDto>();
                    foreach (var cartLine in data.CartTaxItems)
                    {
                        SIDtlTax.Add(new SODtlTaxDto
                        {
                            SOId = existingSo.Id,
                            TaxAmount = cartLine.TaxAmount,
                            AfterTaxAmount = cartLine.AfterTaxAmount,
                            TaxId = 0 // or your real tax ID
                        });
                    }
                    existingSo.SODtlTaxDtos = SIDtlTax;

                    // Finally update
                    await _SORepository.UpdateTransactionAsync(existingSo);

                    return Json(new { success = true, message = "Hold order updated!", newId = existingSo.Id });
                }
                else
                {
                    // -------------- CREATE NEW HOLD --------------
                    var so = new SODto
                    {
                        SOCode = await _tenantPrefixService.InitializeGenerateCode(TenantId, CompanyId, "", PrefixType.SO, true),
                        SODate = DateTime.Now,
                        CustomerId = data.CustomerId,
                        SOSubTotal = data.SubTotal,
                        SODiscount = data.DiscountAmount,
                        SOTotal = data.Total,
                        SOTotalAfterVAT = data.Total,
                        TenantId = TenantId,
                        CompanyId = CompanyId,
                        FinancialYearId = 1,
                        isholdPOS = true // Mark as "on hold"
                    };

                    // Child lines
                    var soDetails = new List<SODtlDto>();
                    foreach (var line in data.CartItems)
                    {
                        soDetails.Add(new SODtlDto
                        {
                            SOId = so.Id, // EF sets later
                            ProductId = line.Id,
                            SODtlQty = line.Qty,
                            SODtlPrice = line.ProductPrice,
                            SODtlTotal = line.Qty * line.ProductPrice
                        });
                    }
                    so.SODtlDtos = soDetails;

                    // Tax lines
                    var SIDtlTax = new List<SODtlTaxDto>();
                    foreach (var cartLine in data.CartTaxItems)
                    {
                        SIDtlTax.Add(new SODtlTaxDto
                        {
                            SOId = so.Id,
                            TaxAmount = cartLine.TaxAmount,
                            AfterTaxAmount = cartLine.AfterTaxAmount,
                            TaxId = 0 // or your real tax ID
                        });
                    }
                    so.SODtlTaxDtos = SIDtlTax;

                    // Insert new
                    await _SORepository.AddTransactionAsync(so);

                    return Json(new { success = true, message = "Order placed on hold.", newId = so.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ----------------------------------------------------------------
        // 3) GetHoldOrders => returns SODto with isholdPOS = true
        // ----------------------------------------------------------------
        [HttpGet]
        public IActionResult GetHoldOrders()
        {
            try
            {
                var holdOrders = _SORepository.GetAllHoldPOSByTenantCompanyAsync(TenantId, CompanyId).Result;
                return Json(new { success = true, data = holdOrders });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ----------------------------------------------------------------
        // 4) GetHoldOrderDetails => returns cart lines from the SODtl
        // ----------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetHoldOrderDetails(int soId)
        {
            try
            {
                var so = await _SORepository.GetByIdAsync(soId);
                if (so == null)
                    return Json(new { success = false, message = "Hold order not found." });

                var dtlList = await _SODtlRepository.GetAllBySOIdAsync(so.Id);
                var cartItems = dtlList.Select(d => new
                {
                    Id = d.ProductId,
                    ProductName = d.ProductName, // if you want to fetch from Product table, do so
                    ProductPrice = d.SODtlPrice,
                    Qty = d.SODtlQty,
                }).ToList();

                var dtlTaxList = await _SODtlTaxRepository.GetAllBySOIdAsync(so.Id);
                var CartTaxItems = dtlTaxList.Select(t => new PosCartTaxItemVM
                {
                    ProductId = 0,      // Directly from t, if available
                    TaxAmount = t.TaxAmount,
                    AfterTaxAmount = t.AfterTaxAmount,
                    TaxId = t.TaxId
                }).ToList();

                var data = new
                {
                    soId = so.Id,
                    customerId = so.CustomerId,
                    discount = so.SODiscount,
                    total = so.SOTotal,
                    isholdPOS = true,
                    cartItems,
                    CartTaxItems
                };

                return Json(new { success = true, data= data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
