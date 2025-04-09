using AutoMapper;
using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.ExternalModel;
using System.Data.Common;
using System.Formats.Asn1;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using WebdeskEA.Domain.CommonMethod;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class ProjectPermissionRepository : Repository<Module>, IProjectPermissionRepository
    {
        private readonly IMapper _mapper;

        public ProjectPermissionRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ SideBar Permission -------
        #region SideBar_Permission
        //old Sidebar
        public async Task<IEnumerable<ModuleDto>> GetUserSideBarOnlyFormMenuByUserId(string UserId)
        {
            var procedure = "spSideBarMenu";
            var parameters = new { UserId = UserId };
            var result = await _dbConnection.QueryAsync<Module>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ModuleDto>>(result);
        }
        //updatedSiderbar base on Tenant
        public async Task<IEnumerable<ModuleDto>> GetPermTenantUserSideBarByUserId(string UserId)
        {
            var procedure = "spGetPermTenantUserSideBarByUserId";
            var parameters = new { UserId = UserId };
            var result = await _dbConnection.QueryAsync<Module>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ModuleDto>>(result);
        }
        #endregion


        //------ Role_Module_Rights Permission -------
        #region Role_Module_Rights

        //Old Permission
        #region Old
        public async Task<IEnumerable<UserRightDto>> GetAllListByUserIdAsync(string UserId)
        {
            var procedure = "spUserRights_GetAllByUserId";
            var parameters = new { User = UserId };
            var result = await _dbConnection.QueryAsync<UserRight>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(result);
        }
        public async Task<IEnumerable<UserRightDto>> GetPermissionListByUserIdAsync(string UserId)
        {
            var procedure = "spUserRights_GetAllByUserId";
            var parameters = new { User = UserId };
            var result = await _dbConnection.QueryAsync<UserRight>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(result);
        }
        public async Task<IEnumerable<UserRightDto>> TenantBasePermissiongModuleRolesAsync(string UserId)
        {
            var procedure = "spTenantBasePermissiongModuleRoles_GetAllByUserId";
            var parameters = new { User = UserId };
            var result = await _dbConnection.QueryAsync<UserRight>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<UserRightDto>>(result);
        }
        /// <summary>
        /// for Modules Permission checking during login so its open the connection string and fetch the permision base on module
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<string>> GetPermissionsByModuleIdAsync(int moduleId)
        {
            var procedure = "spGetPermissionsByModuleId";
            var parameters = new { ModuleId = moduleId };

            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }
                var permissions = await _dbConnection.QueryAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing the query.", ex);
            }
            finally
            { }
        }
        public async Task<IEnumerable<string>> GetExistingPermissinDyanmicListAsync()
        {
            var procedure = "spGetExistingPermissionListDynamic";
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }
                var permissions = await _dbConnection.QueryAsync<string>(procedure, commandType: CommandType.StoredProcedure);
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing the query.", ex);
            }
            finally
            { }
        }
        #endregion

        //Tenant Permission
        #region Tenant
        public async Task<IEnumerable<RolePermissionDto>> GetPermTenantListByUserIdAsync(string UserId)
        {
            var procedure = "spGetPermTenantListByUserId";
            var parameters = new { UserId = UserId };
            var result = await _dbConnection.QueryAsync<RolePermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<RolePermissionDto>>(result);
        }
        #endregion

        #endregion


        #region  AllPoliciy_Roles_Permission
        public async Task<string> GetAllRolePoliciyPermissionAsync()
        {
            var procedure = "spRole_GetAllRolePolicy";
            var roles = await _dbConnection.QueryAsync<Role>(procedure, commandType: CommandType.StoredProcedure);
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return string.Join(",", roleDtos.Select(role => role.RoleName)); // Assuming `RoleDto` has a `Name` property
        }
        #endregion



        #region Not_In_Use
        public Task<RolePermissionDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<RolePermissionDto>> IProjectPermissionRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
