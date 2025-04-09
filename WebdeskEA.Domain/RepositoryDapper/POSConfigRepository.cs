using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper
{
    using AutoMapper;
    using Dapper;
    using System.Data;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using WebdeskEA.Models.MappingModel;
    using WebdeskEA.Domain.CommonMethod;

    public class POSConfigRepository : Repository<POSConfig>, IPOSConfigRepository
    {
        private readonly IMapper _mapper;
        public POSConfigRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        // Get POSConfig by Id
        public async Task<POSConfigDto> GetByIdAsync(int id)
        {
            var procedure = "spPOSConfig_GetById";
            var parameters = new { Id = id };
            var POSConfig = await _dbConnection.QueryFirstOrDefaultAsync<POSConfigDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<POSConfigDto>(POSConfig);
        }

        // Get POSConfig by Id
        public async Task<POSConfigDto> GetByIdTenantIdCompanyIdAsync(int id, int TenantId, int CompanyId)
        {
            var procedure = "spPOSConfig_GetByIdTenantIdCompanyId";
            var parameters = new { Id = id, TenantId = TenantId, CompanyId = CompanyId };
            var POSConfig = await _dbConnection.QueryFirstOrDefaultAsync<POSConfigDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<POSConfigDto>(POSConfig);
        }

        public async Task<POSConfigDto> GetByActiveTenantIdCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPOSConfig_GetByActiveTenantIdCompanyId";
            var parameters = new {  TenantId = TenantId, CompanyId = CompanyId };
            var POSConfig = await _dbConnection.QueryFirstOrDefaultAsync<POSConfigDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<POSConfigDto>(POSConfig);
        }

        // Get all POSConfigs
        public async Task<IEnumerable<POSConfigDto>> GetAllAsync()
        {
            var procedure = "spPOSConfig_GetAll";
            var POSConfigs = await _dbConnection.QueryAsync<POSConfig>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POSConfigDto>>(POSConfigs);
        }

        public async Task<IEnumerable<POSConfigDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPOSConfig_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, CompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<POSConfig>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POSConfigDto>>(Banks);
        }

        // Get POSConfigs by POSConfig
        public async Task<IEnumerable<POSConfigDto>> GetByPOSConfigAsync(int POSConfigId)
        {
            var procedure = "GetPOSConfigsByPOSConfig";
            var parameters = new { POSConfigId = POSConfigId };
            var POSConfigs = await _dbConnection.QueryAsync<POSConfig>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POSConfigDto>>(POSConfigs);
        }

        // Bulk load POSConfigs
        public async Task<IEnumerable<POSConfigDto>> BulkLoadPOSConfigsAsync(string procedure, object parameters = null)
        {
            var POSConfigs = await _dbConnection.QueryAsync<POSConfigDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POSConfigDto>>(POSConfigs);
        }

        // Get paginated POSConfigs
        public async Task<IEnumerable<POSConfigDto>> GetPaginatedPOSConfigsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "POSConfig_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var POSConfigs = await _dbConnection.QueryAsync<POSConfigDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<POSConfigDto>>(POSConfigs);
        }

        #endregion

        #region Add Method

        // Add a new POSConfig
        public async Task<int> AddAsync(POSConfigDto POSConfigDto)
        {
            var procedure = "spPOSConfig_Insert";
            var entity = _mapper.Map<POSConfig>(POSConfigDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        // Bulk insert POSConfigs
        public async Task<int> BulkInsertPOSConfigsAsync(IEnumerable<POSConfigDto> POSConfigDtos)
        {
            var procedure = "POSConfig_BulkInsert";
            var POSConfigs = _mapper.Map<IEnumerable<POSConfig>>(POSConfigDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = POSConfigs }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing POSConfig
        public async Task<int> UpdateAsync(POSConfigDto POSConfigDto)
        {
            try
            {
                var procedure = "spPOSConfig_Update";
                var entity = _mapper.Map<POSConfig>(POSConfigDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region Delete Method
        // Delete a POSConfig by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPOSConfig_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
