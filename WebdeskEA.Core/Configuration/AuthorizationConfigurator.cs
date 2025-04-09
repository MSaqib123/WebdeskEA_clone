using WebdeskEA.Domain.RepositoryDapper.IRepository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Core.Configuration
{

    public class AuthorizationConfigurator : IAuthorizationConfigurator
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;
        private readonly PermissionDefinitions _permissionDefinitions;

        public AuthorizationConfigurator(IModuleRepository moduleRepository,
            PermissionDefinitions permissionDefinitions,
            IProjectPermissionRepository projectPermissionRepository)
        {
            _moduleRepository = moduleRepository;
            _permissionDefinitions = permissionDefinitions;
            _ProjectPermissionRepository = projectPermissionRepository;
        }

        public async Task ConfigurePolicies(IServiceCollection services)
        {
            var modules = await _moduleRepository.GetAllAsync();

            var modulePermissionsTasks = modules.Select(async module =>
            {
                //______ Fetch All Existing Permision _____
                var permissions = await _ProjectPermissionRepository.GetExistingPermissinDyanmicListAsync();
                var permissionList = permissions.ToList();
                return new
                {
                    ModuleName = module.ModuleName,
                    Permissions = permissionList
                };
            });

            var modulePermissionsList = await Task.WhenAll(modulePermissionsTasks);

            services.AddAuthorization(async options =>
            {
                foreach (var module in modulePermissionsList)
                {
                    string moduleName = module.ModuleName;

                    foreach (var permission in module.Permissions)
                    {
                        string policyName = $"{moduleName}{permission}";

                        // Ensure permissions are mapped to their indices and policies are created
                        var permissionIndex = await _permissionDefinitions.MapPermissionToIndexAsync(permission);
                        options.AddPolicy(policyName, policy =>
                            policy.Requirements.Add(new PermissionRequirement($"{moduleName}_Permissions", permissionIndex)));
                    }
                }
            });
        }
    }



    //public class AuthorizationConfigurator
    //{
    //    private readonly IModuleRepository _moduleRepository;
    //    private readonly IUserRightsRepository _userRightsRepository;
    //    public AuthorizationConfigurator(IModuleRepository moduleRepository, IUserRightsRepository userRightsRepository)
    //    {
    //        _moduleRepository = moduleRepository;
    //        _userRightsRepository = userRightsRepository;
    //    }

    //    public async Task ConfigurePolicies(IServiceCollection services)
    //    {
    //        var modules = await _moduleRepository.GetAllListAsync();

    //        // Fetch permissions for each module
    //        var modulePermissionsTasks = modules.Select(async module =>
    //        {
    //            var permissions = await _userRightsRepository.GetPermissionsByModuleIdAsync(module.Id);
    //            var permissionList = permissions.ToList();
    //            return new
    //            {
    //                ModuleName = module.ModuleName,
    //                Permissions = permissionList
    //            };
    //        });

    //        var modulePermissionsList = await Task.WhenAll(modulePermissionsTasks);

    //        services.AddAuthorization(options =>
    //        {
    //            foreach (var module in modulePermissionsList)
    //            {
    //                string moduleName = module.ModuleName;
    //                List<string> permissions = module.Permissions;

    //                for (int i = 0; i < permissions.Count; i++)
    //                {
    //                    string permission = permissions[i];
    //                    string policyName = $"{moduleName}{permission}";

    //                    // Create a policy requirement where the permission index is passed
    //                    options.AddPolicy(policyName, policy =>
    //                        policy.Requirements.Add(new PermissionRequirement($"{moduleName}_Permissions", i)));
    //                }
    //            }
    //        });
    //    }
    //}
}
