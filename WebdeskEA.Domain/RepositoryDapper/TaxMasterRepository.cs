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
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.CommonMethod;


namespace WebdeskEA.Domain.RepositoryDapper
{
    
    public class TaxMasterRepository : Repository<TaxMaster>, ITaxMasterRepository
    {
        private readonly IMapper _mapper;
        public TaxMasterRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        // Get TaxMaster by Id
        public async Task<TaxMasterDto> GetByIdAsync(int id)
        {
            var procedure = "spTaxMaster_GetById";
            var parameters = new { Id = id };
            var TaxMaster = await _dbConnection.QueryFirstOrDefaultAsync<TaxMasterDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<TaxMasterDto>(TaxMaster);
        }
        public async Task<TaxMasterDto> GetByIdTenantId(int id,int TenantId)
        {
            var procedure = "spTaxMaster_GetByIdTenantId";
            var parameters = new { Id = id, TenantId = TenantId };
            var TaxMaster = await _dbConnection.QueryFirstOrDefaultAsync<TaxMasterDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<TaxMasterDto>(TaxMaster);
        }

        // Get all TaxMasters
        public async Task<IEnumerable<TaxMasterDto>> GetAllAsync()
        {
            var procedure = "spTaxMaster_GetAll";
            var TaxMasters = await _dbConnection.QueryAsync<TaxMaster>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TaxMasterDto>>(TaxMasters);
        }

        public async Task<IEnumerable<TaxMasterDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spTaxMaster_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<TaxMaster>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TaxMasterDto>>(Banks);
        }

        // Get TaxMasters by category
        public async Task<IEnumerable<TaxMasterDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetTaxMastersByCategory";
            var parameters = new { CategoryId = categoryId };
            var TaxMasters = await _dbConnection.QueryAsync<TaxMaster>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TaxMasterDto>>(TaxMasters);
        }

        // Bulk load TaxMasters
        public async Task<IEnumerable<TaxMasterDto>> BulkLoadTaxMastersAsync(string procedure, object parameters = null)
        {
            var TaxMasters = await _dbConnection.QueryAsync<TaxMasterDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TaxMasterDto>>(TaxMasters);
        }



        // Get paginated TaxMasters
        public async Task<IEnumerable<TaxMasterDto>> GetPaginatedTaxMastersAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "TaxMaster_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var TaxMasters = await _dbConnection.QueryAsync<TaxMasterDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<TaxMasterDto>>(TaxMasters);
        }

        #endregion

        #region Add Method

        // Add a new TaxMaster
        public async Task<int> AddAsync(TaxMasterDto TaxMasterDto)
        {
            var procedure = "spTaxMaster_Insert";
            var entity = _mapper.Map<TaxMaster>(TaxMasterDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        // Bulk insert TaxMasters
        public async Task<int> BulkInsertTaxMastersAsync(IEnumerable<TaxMasterDto> TaxMasterDtos)
        {
            var procedure = "TaxMaster_BulkInsert";
            var TaxMasters = _mapper.Map<IEnumerable<TaxMaster>>(TaxMasterDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = TaxMasters }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing TaxMaster
        public async Task<int> UpdateAsync(TaxMasterDto TaxMasterDto)
        {
            try
            {
                var procedure = "spTaxMaster_Update";
                var entity = _mapper.Map<TaxMaster>(TaxMasterDto);
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
        // Delete a TaxMaster by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spTaxMaster_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
