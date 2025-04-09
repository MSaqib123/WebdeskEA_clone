using AutoMapper;
using Dapper;
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

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class TenantPermissionRepository : Repository<TenantPermission>, ITenantPermissionRepository
    {
        private readonly IMapper _mapper;

        public TenantPermissionRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        //------ Get -------
        #region Get
        public async Task<TenantPermissionDto> GetByIdAsync(int id)
        {
            var procedure = "spTenantPermission_GetById";
            var parameters = new { Id = id };
            var tenantPermission = await _dbConnection.QueryFirstOrDefaultAsync<TenantPermission>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<TenantPermissionDto>(tenantPermission);
        }

        public async Task<IEnumerable<TenantPermissionDto>> GetAllAsync()
        {
            var procedure = "spTenantPermission_GetAll";
            var tenantPermissions = await _dbConnection.QueryAsync<TenantPermission>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TenantPermissionDto>>(tenantPermissions);
        }
        #endregion

        //------ Add -------
        #region Add
        public async Task<int> AddAsync(TenantPermissionDto dto)
        {
            var procedure = "spTenantPermission_Insert";
            var entity = _mapper.Map<TenantPermission>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);

            // Output parameter for the newly inserted Id
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Execute stored procedure
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);

            // Check if @Id is null (DBNull), and return -1 or handle accordingly
            var insertedId = parameters.Get<int?>("@Id");
            if (!insertedId.HasValue)
            {
                throw new InvalidOperationException("Failed to insert TenantPermission: Id returned as null.");
            }

            return insertedId.Value;  // Return the actual Id
        }

        #endregion

        //------ Update -------
        #region Update
        public async Task<int> UpdateAsync(TenantPermissionDto dto)
        {
            var procedure = "spTenantPermission_Update";
            var entity = _mapper.Map<TenantPermission>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        //------ Delete -------
        #region Delete
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spTenantPermission_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteByTenantIdAsync(int TenantId)
        {
            try
            {
                var procedure = "spTenantPermission_DeleteByTenant";
                var parameters = new { TenantId = TenantId };
                return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }
        #endregion
    }

}
