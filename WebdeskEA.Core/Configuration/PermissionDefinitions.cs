using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Core.Configuration
{
    public class PermissionDefinitions
    {
        private readonly IUserRightsRepository _permissionRepo;
        private readonly IProjectPermissionRepository _ProjectPermissionRepository;

        // Constructor with dependency injection
        public PermissionDefinitions(
            IUserRightsRepository permissionRepo, 
            IProjectPermissionRepository projectPermissionRepository)
        {
            _permissionRepo = permissionRepo;
            _ProjectPermissionRepository = projectPermissionRepository;
        }

        // Fetches permissions from the repository dynamically
        private async Task<string[]> GetPermissionsAsync()
        {
            var permissions = await _ProjectPermissionRepository.GetExistingPermissinDyanmicListAsync();
            //Console.WriteLine("Permissions fetched from table:");
            //foreach (var permission in permissions)
            //{
            //    //Console.WriteLine(permission);
            //}
            return permissions.ToArray();
        }

        // Maps permission names to their bitmask indices
        public async Task<int> MapPermissionToIndexAsync(string permission)
        {
            var permissions = await GetPermissionsAsync();
            int index = Array.IndexOf(permissions, permission);
            if (index == -1)
            {
                throw new ArgumentException($"Invalid permission: {permission}", nameof(permission));
            }
            return index;
        }

        // Packs permissions into a bitmask by dynamically checking UserRightDto properties
        public async Task<int> PackPermissionsAsync(RolePermissionDto right)
        {
            var permissions = await GetPermissionsAsync();
            int packed = 0;

            foreach (var permission in permissions)
            {
                var propertyInfo = right.GetType().GetProperty($"Perm{permission}");
                if (propertyInfo != null)
                {
                    bool hasPermission = (bool)propertyInfo.GetValue(right);
                    //Console.WriteLine($"Checking permission: {permission}, Has Permission: {hasPermission}");
                    packed |= hasPermission ? 1 << await MapPermissionToIndexAsync(permission) : 0;  //0:Delete ,1:Email ,2:Insert, 3:Noti , 4:Print , 5:Update , 6:View
                }
                else
                {
                    //Console.WriteLine($"No matching property found for permission: {permission}");
                }
            }
            return packed;
        }
    }



    //public static class PermissionDefinitions
    //{
    //    // Define permission names
    //    public static readonly string[] Permissions = new[]
    //    {
    //        "Insert",
    //        "Update",
    //        "View",
    //        "Delete",
    //        "Print",
    //        "Edit",
    //        "Approve"
    //    };

    //    // Map permission names to bitmask indices
    //    public static int MapPermissionToIndex(string permission)
    //    {
    //        int index = Array.IndexOf(Permissions, permission);
    //        if (index == -1)
    //        {
    //            throw new ArgumentException("Invalid permission", nameof(permission));
    //        }
    //        return index;
    //    }

    //    // Pack permissions into a bitmask
    //    public static int PackPermissions(UserRightDto right)
    //    {
    //        int packed = 0;
    //        packed |= right.RightInsert ? 1 << MapPermissionToIndex("Insert") : 0;
    //        packed |= right.RightUpdate ? 1 << MapPermissionToIndex("Update") : 0;
    //        packed |= right.RightView ? 1 << MapPermissionToIndex("View") : 0;
    //        packed |= right.RightDelete ? 1 << MapPermissionToIndex("Delete") : 0;
    //        packed |= right.RightPrint ? 1 << MapPermissionToIndex("Print") : 0;
    //        packed |= right.RightEdit ? 1 << MapPermissionToIndex("Edit") : 0;
    //        packed |= right.RightApprove ? 1 << MapPermissionToIndex("Approve") : 0;
    //        return packed;
    //    }
    //}
}
