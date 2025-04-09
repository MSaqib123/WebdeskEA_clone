using WebdeskEA.DataAccess;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Core.Configuration
{

    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly WebdeskEADBContext _context;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;
        private readonly IModuleRepository _modulesRepo;
        private readonly PermissionDefinitions _permissionDefinitions;

        public CustomClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            WebdeskEADBContext context,
            PermissionDefinitions permissionDefinitions,
            IOptions<IdentityOptions> optionsAccessor,
            IModuleRepository modulesRepo,
            IProjectPermissionRepository projectPermissionRepository) : base(userManager, roleManager, optionsAccessor)
        {
            _context = context;
            _modulesRepo = modulesRepo;
            _permissionDefinitions = permissionDefinitions;
            _ProjectPermissionRepository = projectPermissionRepository;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            //1. Ading Permssion by bitmask Pack
            #region Permission_Module
            var principal = await base.CreateAsync(user);
            var identity = principal.Identity as ClaimsIdentity ?? throw new InvalidOperationException("Claims identity is null");
            var userRights = await _ProjectPermissionRepository.GetPermTenantListByUserIdAsync(user.Id);
            var modules = await _modulesRepo.GetAllAsync();
            foreach (var right in userRights)
            {
                var module = modules.FirstOrDefault(m => m.Id == right.ModuleId && m.Active);
                if (module != null)
                {
                    await AddClaimsForModuleAsync(identity, module.ModuleName, right);
                }
            }
            #endregion

            //2. Ading Roles
            #region Roles
            var distinctRoles = userRights
                .Where(r => !string.IsNullOrEmpty(r.RoleName))  // Filter only valid roles
                .Select(r => r.RoleName)
                .Distinct();  // Avoid duplicates

            foreach (var role in distinctRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role ?? "Guest"));
            }
            #endregion

            return principal;
        }

        private async Task AddClaimsForModuleAsync(ClaimsIdentity identity, string moduleName, RolePermissionDto right)
        {
            int packedPermissions = await _permissionDefinitions.PackPermissionsAsync(right);
            //Console.WriteLine($"Adding Claim: {moduleName}_Permissions = {packedPermissions}");
            identity.AddClaim(new Claim($"{moduleName}_Permissions", packedPermissions.ToString()));
        }
    }

    //public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>, ICustomClaimsPrincipalFactory
    //{
    //    private readonly WebdeskEADBContext _context;
    //    private readonly IUserRightsRepository _userRightsRepo;
    //    private readonly IModuleRepository _ModulesRepo;

    //    public CustomClaimsPrincipalFactory(
    //        UserManager<ApplicationUser> userManager,
    //        IUserRightsRepository userRightsRepo,
    //        IModuleRepository ModulesRepo,
    //        RoleManager<IdentityRole> roleManager,
    //        IOptions<IdentityOptions> optionsAccessor,
    //        WebdeskEADBContext context
    //        )
    //        : base(userManager, roleManager, optionsAccessor)
    //    {
    //        _context = context;
    //        _userRightsRepo = userRightsRepo;
    //        _ModulesRepo = ModulesRepo;
    //    }

    //    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    //    {
    //        var principal = await base.CreateAsync(user);
    //        var identity = principal.Identity as ClaimsIdentity ?? throw new InvalidOperationException("Claims identity is null");

    //        // Fetch user rights and modules dynamically from the database
    //        var userRights = await _userRightsRepo.GetAllListByUserIdAsync(user.Id);
    //        var modules = await _ModulesRepo.GetAllListAsync();

    //        foreach (var right in userRights.ToList())
    //        {
    //            var module = modules.FirstOrDefault(m => m.Id == right.ModuleId && m.Active);
    //            if (module != null)
    //            {
    //                AddClaimsForModule(identity, module.ModuleName, right);
    //            }
    //        }

    //        return principal;
    //    }

    //    private void AddClaimsForModule(ClaimsIdentity identity, string moduleName, UserRightDto right)
    //    {
    //        if (right.RightView)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_View"));
    //        }
    //        if (right.RightInsert)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Insert"));
    //        }
    //        if (right.RightEdit)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Edit"));
    //        }
    //        if (right.RightUpdate)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Update"));
    //        }
    //        if (right.RightDelete)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Delete"));
    //        }
    //        if (right.RightApprove)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Approve"));
    //        }
    //        if (right.RightPrint)
    //        {
    //            identity.AddClaim(new Claim(ClaimTypes.Role, $"{moduleName}_Print"));
    //        }
    //    }
    //}
}
