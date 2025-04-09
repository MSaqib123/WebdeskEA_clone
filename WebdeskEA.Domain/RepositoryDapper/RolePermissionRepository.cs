using AutoMapper;
using Dapper;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using static Dapper.SqlMapper;


namespace WebdeskEA.Domain.RepositoryDapper
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        private readonly IMapper _mapper;

        public RolePermissionRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<RolePermissionDto> GetByIdAsync(int id)
        {
            var procedure = "spRolePermission_GetById";
            var parameters = new { Id = id };
            var rolePermission = await _dbConnection.QueryFirstOrDefaultAsync<RolePermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<RolePermissionDto>(rolePermission);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllAsync()
        {
            var procedure = "spRolePermission_GetAll";
            var rolePermissions = await _dbConnection.QueryAsync<RolePermission>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<RolePermissionDto>>(rolePermissions);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllByRoleIdAsync(int RoleId)
        {
            var procedure = "spRolePermission_GetAllByRoleIdAsync";
            var parameters = new { RoleId = RoleId };
            var rolePermissions = await _dbConnection.QueryAsync<RolePermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<RolePermissionDto>>(rolePermissions);
        }

        #endregion

        #region Add Method

        public async Task<int> AddAsync(RolePermissionDto rolePermissionDto)
        {
            var procedure = "spRolePermission_Insert";
            var entity = _mapper.Map<RolePermission>(rolePermissionDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }

        public async Task<int> BulkAddAsync(RolePermissionDto rolePermissionDto)
        {
            //=== Delete all role ====
            await DeleteByRoleIdAsync(rolePermissionDto!.RoleId!.Value);

            foreach (var item in rolePermissionDto.rolePermission)
            {
                if (
                    item.PermInsert == true ||
                    item.PermUpdate == true ||
                    item.PermView == true ||
                    item.PermPrint == true ||
                    item.PermDelete == true ||
                    item.PermEmail == true ||
                    item.PermNotification == true
                    )
                {
                    item.RoleId = rolePermissionDto.RoleId;
                    var procedure = "spRolePermission_Insert";
                    var entity = _mapper.Map<RolePermission>(item);
                    var parameters = CommonDapperMethod.GenerateParameters(entity);
                    parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            return 1;
        }


        #endregion

        #region Update Method

        public async Task<int> UpdateAsync(RolePermissionDto rolePermissionDto)
        {
            var procedure = "spRolePermission_Update";
            var entity = _mapper.Map<RolePermission>(rolePermissionDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Delete Method

        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spRolePermission_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByRoleIdAsync(int RoleId)
        {
            var procedure = "spRolePermission_DeleteByRoleId";
            var parameters = new { RoleId = RoleId };
            int result = await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            if (result > 0)
            {
                return result;
            }
            else
            {
                return 1;
            }
            

        }

        #endregion
    }

}
