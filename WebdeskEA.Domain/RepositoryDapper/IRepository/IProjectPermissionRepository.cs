using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IProjectPermissionRepository
    {
        //================ SideBar Permission ===============
        #region SideBarPermission
        Task<IEnumerable<ModuleDto>> GetUserSideBarOnlyFormMenuByUserId(string UserId);
        Task<IEnumerable<ModuleDto>> GetPermTenantUserSideBarByUserId(string UserId);
        #endregion

        //================ Role_Module_Rights  Permission ===============
        #region Role_Module_Rights
        #region old
        Task<IEnumerable<UserRightDto>> GetAllListByUserIdAsync(string UserId);
        Task<IEnumerable<UserRightDto>> GetPermissionListByUserIdAsync(string UserId);
        Task<IEnumerable<UserRightDto>> TenantBasePermissiongModuleRolesAsync(string UserId);
        Task<IEnumerable<string>> GetPermissionsByModuleIdAsync(int moduleId);
        Task<IEnumerable<string>> GetExistingPermissinDyanmicListAsync();
        #endregion

        Task<IEnumerable<RolePermissionDto>> GetPermTenantListByUserIdAsync(string UserId);
        #endregion


        //================ Subscription_Permission ===============
        #region Subscription_Permission

        #endregion


        //================ NotINUsed ===============
        #region Not_In_Used
        Task<RolePermissionDto> GetByIdAsync(int id);
        Task<IEnumerable<RolePermissionDto>> GetAllAsync();
        Task<string> GetAllRolePoliciyPermissionAsync();
        #endregion
    }

}
