using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Security.Claims;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Domain.Service;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Models.Utility;
using WebdeskEA.Models.Utility.EnumUtality;
using static Dapper.SqlMapper;
using MySessionExtensions = WebdeskEA.Core.Extension.SessionExtensions;
namespace CRM.Areas.Accounts.Controllers
{
    [Area("Common")]
    public class DropDownController : Controller
    {
        //=================== Constructor =====================
        #region Constructor
        protected int TenantId => Convert.ToInt32(MySessionExtensions.TenantId(HttpContext) ?? 0);
        protected int CompanyId => Convert.ToInt32(MySessionExtensions.CompanyId(HttpContext) ?? 0);
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly ICOARepository _Coa;
        private readonly ICOATypeRepository _CoaType;
        private readonly IEnumService _enumService;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly ICompanyRepository _CompanyRepository;
        private readonly ICompanyBusinessCategoryRepository _companyBusinessCategoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateProvinceRepository _stateProvinceRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly UserManager<ApplicationUser> _userManager; // Ensure your user class is specified
        private readonly SignInManager<ApplicationUser> _signInManager; // En

        public DropDownController(ICompanyUserRepository companyUserRepository,
            IErrorLogRepository errorLogRepository,
            ICompanyRepository companyRepository,
            ICompanyBusinessCategoryRepository companyBusinessCategoryRepository,
            ICOARepository Coa,
            ICOATypeRepository CoaType,
            IEnumService enumService,
            ICityRepository cityRepository,
            ICountryRepository countryRepository,
            IStateProvinceRepository stateProvinceRepository,
            ICustomerRepository customerRepository,
            ISupplierRepository supplierRepository,
            IBrandRepository brandRepository,
            IFinancialYearRepository financialYearRepository,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _companyUserRepository = companyUserRepository;
            _enumService = enumService;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
            _CompanyRepository = companyRepository;
            _companyBusinessCategoryRepository = companyBusinessCategoryRepository;
            _Coa = Coa;
            _CoaType = CoaType;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _brandRepository = brandRepository;
            _financialYearRepository = financialYearRepository;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NameOfForm = "Chart of Account";
            base.OnActionExecuting(context);
        }

        #endregion

        //=================== Partial Binding =====================
        #region Partial_Binding                                                                        

        //------- Location_Combo ------
        #region Location_Combo
        [HttpGet]
        public async Task<JsonResult> GetCountries()
        {
            try
            {
                var entities = await _countryRepository.GetAllAsync();
                var result = entities.Select(x => new { id = x.Id, name = x.CountryName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
            
        }

        [HttpGet]
        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            try
            {
                var entities = await _stateProvinceRepository.GetStateProvinceByCountryIdAsync(countryId);
                var result = entities.Select(x => new { id = x.Id, name = x.StateProvinceName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
            
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByState(int stateId)
        {
            try
            {
                var entities = await _cityRepository.GetCityByStateIdAsync(stateId);
                var result = entities.Select(x => new { id = x.Id, name = x.CityName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
            
        }
        #endregion

        //------- COA_Drop ------
        #region COA_Drop
        [HttpGet]
        public async Task<JsonResult> GetAllCOA()
        {
            try
            {
                var entities = await _Coa.GetAllByTenantCompanyIdAsync(TenantId, CompanyId);
                var result = entities.Select(x => new { id = x.Id, name = x.AccountName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCoaBySupplier(int SupplierId)
        {
            try
            {
                //--- CustomerList ----
                var supplierDto = await _supplierRepository.GetByIdAsync(SupplierId);

                //--- ChartOfAccount ---
                var coa = await _Coa.GetByIdAsync(supplierDto.COAId, TenantId, CompanyId);
                var entities = coa != null ? new List<COADto> { coa } : new List<COADto>();

                var result = entities.Select(x => new { id = x.Id, name = x.AccountName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCoaByCustomer(int customerId)
        {
            try
            {
                //--- CustomerList ----
                var customer = await _customerRepository.GetByIdAsync(customerId);

                //--- ChartOfAccount ---
                var coa = await _Coa.GetByIdAsync(customer.COAId, TenantId, CompanyId);
                var entities = coa != null ? new List<COADto> { coa } : new List<COADto>();

                var result = entities.Select(x => new { id = x.Id, name = x.AccountName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCoaByCoaTypeId(int coaTypeId)
        {
            try
            {
                var entities = await _Coa.GetAllByCompanyIdOrAccountTypeIdAsync(TenantId,CompanyId,coaTypeId);
                var result = entities.Select(x => new { id = x.Id, name = x.AccountName }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }

        }
        #endregion

        //------- Update Dropdown ------
        #region Update Dropdown
        [HttpGet]
        public async Task<JsonResult> UpdateCompany(int CompanyId)
        {
            try
            {
                HttpContext.Session.SetInt32("CompanyId", CompanyId);
                var result = true;
                return Json(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                var result = false;
                return Json(result);
            }
        }

        [HttpGet]
        public async Task<JsonResult> UpdateFinancialYear(int id)
        {
            try
            {
                var result = await _financialYearRepository.UpdateIsLocakByCompanyAndFYIdAsync(id,CompanyId);
                if(result > 0)
                {
                    HttpContext.Session.SetInt32("FinancialYearId", id);
                    return Json(new { success = true, message = "Selected successfully"});
                }
                else
                {
                    return Json(new { success = false, message = "Faild to Select" });
                }
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion




        //------- Opening the DropdwonValues Dynamically ------
        #region OpeningDrpdwonValue Dyanamically
        [HttpPost]
        public async Task<IActionResult> OpenDropdwnEntry([FromBody] DynamicRequest requestData)
        {
            try
            {
                // Validate the request
                var validationResult = ValidateRequest(requestData);
                if (!validationResult.IsValid)
                {
                    return Json(new { success = false, message = validationResult.Message });
                }

                // Get the repository service
                var service = GetServiceInstance(requestData.ServiceName);
                if (service == null)
                {
                    return Json(new { success = false, message = $"Service '{requestData.ServiceName}' not found." });
                }

                // Get the DTO type dynamically
                var dtoType = GetDtoType(requestData.DtoName);
                if (dtoType == null)
                {
                    return Json(new { success = false, message = $"DTO type '{requestData.DtoName}' not found." });
                }

                // Create and populate the DTO instance
                var dtoInstance = CreateAndPopulateDto(dtoType, requestData);
                if (dtoInstance == null)
                {
                    return Json(new { success = false, message = "Failed to create and populate DTO instance." });
                }

                // Dynamically invoke the AddAsync method
                var result = await InvokeAddAsync(service, dtoInstance);
                if (result <= 0)
                {
                    return Json(new { success = false, message = "Failed to save data." });
                }
                // Retrieve the ID and return success response
                //var idValue = GetPropertyValue(dtoInstance, requestData.ValueColumn);
                return Json(new { success = true, id = result, name = requestData.Value });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Validates the incoming request.
        /// </summary>
        private (bool IsValid, string Message) ValidateRequest(DynamicRequest requestData)
        {
            if (string.IsNullOrWhiteSpace(requestData.ServiceName) ||
                string.IsNullOrWhiteSpace(requestData.DtoName) ||
                string.IsNullOrWhiteSpace(requestData.ValueColumn) ||
                string.IsNullOrWhiteSpace(requestData.TextColumn) ||
                string.IsNullOrWhiteSpace(requestData.Value))
            {
                return (false, "Invalid request data.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Maps the service name to the actual repository instance.
        /// </summary>
        private object GetServiceInstance(string serviceName)
        {
            return serviceName switch
            {
                "_brand" => _brandRepository,
                "_customer" => _customerRepository,
                "_supplier" => _supplierRepository,
                "_city" => _cityRepository,
                "_country" => _countryRepository,
                "_stateProvince" => _stateProvinceRepository,
                "_company" => _CompanyRepository,
                _ => null
            };
        }

        /// <summary>
        /// Retrieves the DTO type dynamically using reflection.
        /// </summary>
        private Type GetDtoType(string dtoName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName!.Contains("WebdeskEA.Models"));

            return assembly?.GetTypes()
                .FirstOrDefault(t => t.Name == dtoName && t.Namespace == "WebdeskEA.Models.MappingModel");
        }

        /// <summary>
        /// Creates and populates a DTO instance with necessary properties.
        /// </summary>
        private object CreateAndPopulateDto(Type dtoType, DynamicRequest requestData)
        {
            var instance = Activator.CreateInstance(dtoType);
            if (instance == null) return null;

            // Set TextColumn value
            var textProperty = dtoType.GetProperty(requestData.TextColumn);
            textProperty?.SetValue(instance, requestData.Value);

            // Set TenantId and CompanyId
            var tenantIdProperty = dtoType.GetProperty("TenantId");
            tenantIdProperty?.SetValue(instance, TenantId);

            var companyIdProperty = dtoType.GetProperty("CompanyId");
            companyIdProperty?.SetValue(instance, CompanyId);

            return instance;
        }

        /// <summary>
        /// Invokes the AddAsync method dynamically on the repository service.
        /// </summary>
        private async Task<int> InvokeAddAsync(object service, object dtoInstance)
        {
            var addMethod = service.GetType()
                .GetMethod("AddAsync", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (addMethod == null)
            {
                throw new Exception("Method 'AddAsync' not found in the service class.");
            }

            return await (Task<int>)addMethod.Invoke(service, new[] { dtoInstance });
        }

        /// <summary>
        /// Retrieves the value of a specific property from the DTO instance.
        /// </summary>
        private object GetPropertyValue(object instance, string propertyName)
        {
            var property = instance.GetType().GetProperty(propertyName);
            return property?.GetValue(instance);
        }

        public class DynamicRequest
        {
            public string ServiceName { get; set; }
            public string DtoName { get; set; }
            public string ValueColumn { get; set; }
            public string TextColumn { get; set; }
            public string Value { get; set; }
        }

        //[HttpPost]
        //public async Task<IActionResult> OpenDropdwnEntry([FromBody] DynamicRequest requestData)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(requestData.ServiceName) ||
        //            string.IsNullOrWhiteSpace(requestData.DtoName) ||
        //            string.IsNullOrWhiteSpace(requestData.ValueColumn) ||
        //            string.IsNullOrWhiteSpace(requestData.TextColumn) ||
        //            string.IsNullOrWhiteSpace(requestData.Value))
        //        {
        //            return Json(new { success = false, message = "Invalid request data." });
        //        }

        //        // Map service name to actual repository instance
        //        var service = GetServiceInstance(requestData.ServiceName);
        //        if (service == null)
        //        {
        //            return Json(new { success = false, message = $"Service '{requestData.ServiceName}' not found." });
        //        }


        //        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName!.Contains("WebdeskEA.Models"));

        //        if (assembly == null)
        //        {
        //            return Json(new { success = false, message = "DTO assembly not found." });
        //        }


        //        foreach (var type in assembly.GetTypes())
        //        {
        //            Console.WriteLine(type.FullName);
        //            Console.WriteLine(type.Namespace);
        //        }


        //        // Attempt to find the type dynamically
        //        var dtoType = assembly.GetTypes().FirstOrDefault(t => t.Name == requestData.DtoName && t.Namespace == "WebdeskEA.Models.MappingModel");
        //        if (dtoType == null)
        //        {
        //            return Json(new { success = false, message = $"DTO type '{requestData.DtoName}' not found in namespace 'WebdeskEA.Models.MappingModel'." });
        //        }

        //        // Create an instance of the DTO
        //        var dtoInstance = Activator.CreateInstance(dtoType);

        //        var textProperty = dtoType.GetProperty(requestData.TextColumn);
        //        if (textProperty == null)
        //        {
        //            return Json(new { success = false, message = $"Property '{requestData.TextColumn}' not found in DTO." });
        //        }
        //        textProperty.SetValue(dtoInstance, requestData.Value);

        //        // Call the AddAsync method dynamically
        //        var addMethod = service.GetType().GetMethod("AddAsync",BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //        if (addMethod == null)
        //        {
        //            return Json(new { success = false, message = "Method 'AddAsync' not found in the service class." });
        //        }

        //        var result = await (Task<int>)addMethod!.Invoke(service!, new[] { dtoInstance! })!;
        //        if (result > 0)
        //        {
        //            var idProperty = dtoType.GetProperty(requestData.ValueColumn);
        //            if (idProperty == null)
        //            {
        //                return Json(new { success = false, message = $"Property '{requestData.ValueColumn}' not found in DTO." });
        //            }

        //            var idValue = idProperty.GetValue(dtoInstance);
        //            return Json(new { success = true, id = idValue, name = requestData.Value });
        //        }

        //        return Json(new { success = false, message = "Failed to save data." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}

        //// Map service name to repository instance
        //private object GetServiceInstance(string serviceName)
        //{
        //    return serviceName! switch
        //    {
        //        "_brand" => _brandRepository,
        //        "_customer" => _customerRepository,
        //        "_supplier" => _supplierRepository,
        //        "_city" => _cityRepository,
        //        "_country" => _countryRepository,
        //        "_stateProvince" => _stateProvinceRepository,
        //        "_company" => _CompanyRepository,
        //        "_companyBusinessCategory" => _companyBusinessCategoryRepository,
        //        "_coa" => _Coa,
        //        "_coaType" => _CoaType,
        //        _ => null
        //    };
        //}
        #endregion


        //------- Opening the DropdwonValues Dynamically ------
        #endregion

        //=================== Error Logs =====================
        #region ErrorHandling
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
