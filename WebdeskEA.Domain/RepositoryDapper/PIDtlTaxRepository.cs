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
    public class PIDtlTaxRepository : Repository<PIDtlTax>, IPIDtlTaxRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;

        public PIDtlTaxRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper, IProductCOARepository productCOARepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
        }

        #region Get Methods

        // Get product by Id
        public async Task<PIDtlTaxDto> GetByIdAsync(int id)
        {
            var procedure = "spPIDtlTax_GetById";
            var parameters = new { Id = id };
            var product = await _dbConnection.QueryFirstOrDefaultAsync<PIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<PIDtlTaxDto>(product);
        }

        // Get all products
        public async Task<IEnumerable<PIDtlTaxDto>> GetAllAsync()
        {
            var procedure = "spPIDtlTax_GetAll";
            var products = await _dbConnection.QueryAsync<PIDtlTax>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(products);
        }

        public async Task<IEnumerable<PIDtlTaxDto>> GetAllByPIIdAsync(int PIId)
        {
            var procedure = "spPIDtlTax_GetAllByPIId";
            var parameters = new { PIId = PIId };
            var Banks = await _dbConnection.QueryAsync<PIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(Banks);
        }

        public async Task<IEnumerable<PIDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spPIDtlTax_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<PIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(Banks);
        }

        // Get products by category
        public async Task<IEnumerable<PIDtlTaxDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetPIDtlTaxsByCategory";
            var parameters = new { CategoryId = categoryId };
            var products = await _dbConnection.QueryAsync<PIDtlTax>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(products);
        }

        // Bulk load products
        public async Task<IEnumerable<PIDtlTaxDto>> BulkLoadPIDtlTaxsAsync(string procedure, object parameters = null)
        {
            var products = await _dbConnection.QueryAsync<PIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(products);
        }



        // Get paginated products
        public async Task<IEnumerable<PIDtlTaxDto>> GetPaginatedPIDtlTaxsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "PIDtlTax_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var products = await _dbConnection.QueryAsync<PIDtlTaxDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<PIDtlTaxDto>>(products);
        }

        #endregion

        #region Add Method

        // Add a new product
        public async Task<int> AddAsync(PIDtlTaxDto dto, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIDtlTax_Insert";
            var entity = _mapper.Map<PIDtlTax>(dto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> AddTransactionAsync(PIDtlTaxDto dto)
        {
            return 1;
        }


        public async Task<int> BulkAddPIDtlTaxsAsync(IEnumerable<PIDtlTaxDto> productDtos)
        {
            var procedure = "PIDtlTax_BulkInsert";
            var products = _mapper.Map<IEnumerable<PIDtlTaxDto>>(productDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = products }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing product
        public async Task<int> UpdateAsync(PIDtlTaxDto productDto)
        {
            try
            {
                var procedure = "spPIDtlTax_Update";
                var entity = _mapper.Map<PIDtlTax>(productDto);
                var parameters = CommonDapperMethod.GenerateParameters(entity);
                await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTransactionAsync(PIDtlTaxDto dto)
        {
            return 1;
        }

        #endregion

        #region Delete Method
        // Delete a product by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spPIDtlTax_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPIIdAsync(int PIId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            connection ??= _dbConnection;
            var procedure = "spPIDtlTax_DeleteByPIId";
            var parameters = new { PIId = PIId };
            return await connection.ExecuteAsync(procedure, parameters,transaction:transaction, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteByPIDtlIdAsync(int PIDtlId)
        {
            var procedure = "spPIDtlTax_DeleteBySIDtlId";
            var parameters = new { PIId = PIDtlId };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

    }
}
