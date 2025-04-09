using WebdeskEA.DataAccess;
using WebdeskEA.DataAccess.DbInitilizer;
using WebdeskEA.Domain.RepositoryDapper;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Domain.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using Microsoft.Extensions.Configuration;
using WebdeskEA.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using WebdeskEA.DataAccess.StoreProcceduresHandling;
using WebdeskEA.Domain.SecurityService;
using WebdeskEA.Models.Utility.EnumUtality;
using WebdeskEA.Core.Middlware;
using DinkToPdf;
using DinkToPdf.Contracts;
using WebdeskEA.RazerServices;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Core.Extension
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<WebdeskEADBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register IDbConnection with SqlConnection
            services.AddScoped<IDbConnection>(sp =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            });

            // Dapper Connection
            services.AddSingleton<IDapperDbConnectionFactory, DapperDbConnectionFactory>();

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<WebdeskEADBContext>()
                .AddDefaultTokenProviders();

            // Custom Claims Factory
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();

            // Identity Path Configuration
            services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Identity/Account/Login";
                option.AccessDeniedPath = "/Settings/Error/UnAuthorizedAccess";
                option.LogoutPath = "/Identity/Account/Logout";
                option.SlidingExpiration = true;
                option.Cookie.HttpOnly = true;
                option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });


            // Session Registration
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<IRazorRenderer, RazorRenderer>();
            services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

            // Services LifeTime
            services.AddScoped<UserSessionService>();

            services.AddScoped<IStoredProcedureService, StoredProcedureService>();
            services.AddTransient<IDbInitilizer, DbInitilizer>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IEnumService, EnumService>();
            services.AddScoped<WebdeskEA.Domain.RepositoryEntity.IRepository.IApplicationUserRepository, WebdeskEA.Domain.RepositoryEntity.ApplicationUserRepository>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateProvinceRepository, StateProvinceRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
            services.AddScoped<ICompanyBusinessCategoryRepository, CompanyBusinessCategoryRepository>();
            services.AddScoped<ICOARepository, COARepository>();
            services.AddScoped<ICOATypeRepository, COATypeRepository>();
            services.AddScoped<ICOACategoryRepository, COACategoryRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ITenantTypeRepository, TenantTypeRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IUserRightsRepository, UserRightsRepository>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPackageTypeRepository, PackageTypeRepository>();
            services.AddScoped<IPackagePermissionRepository, PackagePermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<ISubscriptionUpgrades_PermisssionsRepository, SubscriptionUpgrades_PermisssionsRepository>();
            services.AddScoped<ITenantPermissionRepository, TenantPermissionRepository>();
            services.AddScoped<IProjectPermissionRepository, ProjectPermissionRepository>();
            services.AddScoped<IFinancialYearRepository, FinancialYearRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IProductCOARepository, ProductCOARepository>();
            services.AddScoped<ITaxMasterRepository, TaxMasterRepository>();
            services.AddScoped<ITenantPrefixService, TenantPrefixService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IOSRepository, OSRepository>();
            services.AddScoped<IOSDtlRepository, OSDtlRepository>();
            services.AddScoped<IOBRepository, OBRepository>();
            services.AddScoped<IOBDtlRepository, OBDtlRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IVoucherDtlRepository, VoucherDtlRepository>();
            services.AddScoped<IVoucherTypeRepository, VoucherTypeRepository>();
            services.AddScoped<IPOSConfigRepository, POSConfigRepository>();

            //=========== Sales ==========
            #region Sales
            services.AddScoped<ISORepository, SORepository>();
            services.AddScoped<ISODtlRepository, SODtlRepository>();
            services.AddScoped<ISODtlTaxRepository, SODtlTaxRepository>();
            services.AddScoped<ISOVATBreakdownRepository, SOVATBreakdownRepository>();

            services.AddScoped<ISIRepository, SIRepository>();
            services.AddScoped<ISIDtlRepository, SIDtlRepository>();
            services.AddScoped<ISIDtlTaxRepository, SIDtlTaxRepository>();
            services.AddScoped<ISIVATBreakdownRepository, SIVATBreakdownRepository>();

            services.AddScoped<ISRRepository, SRRepository>();
            services.AddScoped<ISRDtlRepository, SRDtlRepository>();
            services.AddScoped<ISRVATBreakdownRepository, SRVATBreakdownRepository>();
            services.AddScoped<ISRDtlTaxRepository, SRDtlTaxRepository>();
            #endregion

            //=========== Purchase ==========
            #region Purchase
            services.AddScoped<IPRRepository, PRRepository>();
            services.AddScoped<IPRDtlRepository, PRDtlRepository>();
            services.AddScoped<IPRDtlTaxRepository, PRDtlTaxRepository>();
            services.AddScoped<IPRVATBreakdownRepository, PRVATBreakdownRepository>();

            services.AddScoped<IPIRepository, PIRepository>();
            services.AddScoped<IPIDtlRepository, PIDtlRepository>();
            services.AddScoped<IPIDtlTaxRepository, PIDtlTaxRepository>();
            services.AddScoped<IPIVATBreakdownRepository, PIVATBreakdownRepository>();

            services.AddScoped<IPODtlRepository, PODtlRepository>();
            services.AddScoped<IPORepository, PORepository>();
            services.AddScoped<IPODtlTaxRepository, PODtlTaxRepository>();
            services.AddScoped<IPOVATBreakdownRepository, POVATBreakdownRepository>();
            #endregion

            //=========== Dashboard ==========
            #region Dashboard
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            #endregion

            //=========== Global Setting ==========
            #region GlobalSetting
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            #endregion

            services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
            services.AddScoped<IStockLedgerRepository, StockLedgerRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // TagHelper || RazorCompoennt
            services.AddHttpContextAccessor();
            services.AddAuthorizationCore();
            // Add Razor Pages
            services.AddRazorPages();

            // Permission Policies
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<PermissionDefinitions>();
            services.AddAuthorization();
            services.AddScoped<IAuthorizationConfigurator, AuthorizationConfigurator>();

            // Build ServiceProvider for custom configurator
            //var serviceProvider = services.BuildServiceProvider();
            //var configurator = serviceProvider.GetRequiredService<IAuthorizationConfigurator>();
            //configurator.ConfigurePolicies(services).GetAwaiter().GetResult();
            // Configure Authorization Policy
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var roleService = serviceProvider.GetRequiredService<IProjectPermissionRepository>();
                var roles = roleService.GetAllRolePoliciyPermissionAsync().Result;
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("AllRolesPolicy", policy =>
                        policy.RequireRole(roles.Split(',')));
                });
            }

            return services;
        }
    }
}
