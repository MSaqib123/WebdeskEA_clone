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

    public class ProductCOARepository : Repository<ProductCOA>, IProductCOARepository
    {
        private readonly IMapper _mapper;
        public ProductCOARepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods
        public async Task<ProductCOADto> GetByIdAsync(int id)
        {
            var procedure = "spProductCOA_GetById";
            var parameters = new { Id = id };
            var ProductCOA = await _dbConnection.QueryFirstOrDefaultAsync<ProductCOADto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<ProductCOADto>(ProductCOA);
        }
        public async Task<IEnumerable<ProductCOADto>> GetAllAsync()
        {
            var procedure = "spProductCOA_GetAll";
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOA>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
        }
        public async Task<IEnumerable<int>> GetSelectedSaleCOAsByProductIdAsync(int ProductId)
        {
            var procedure = "spProductCOA_GetSelectedSaleCOAsByProductId";
            var parameters = new { Id = ProductId };
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOA>(procedure, parameters, commandType: CommandType.StoredProcedure);
            var List = _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
            var Ids = List.Where(x=>x.ProductBuyCoaId == 0).Select(x => x.ProductSaleCoaId).ToList();
            return Ids;
        }
        public async Task<IEnumerable<int>> GetSelectedBuyCOAsByProductIdAsync(int ProductId)
        {
            var procedure = "spProductCOA_GetSelectedBuyCOAsByProductId";
            var parameters = new { Id = ProductId };
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOA>(procedure, parameters, commandType: CommandType.StoredProcedure);
            var List = _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
            var Ids = List.Where(x => x.ProductSaleCoaId == 0).Select(x => x.ProductBuyCoaId).ToList();
            return Ids;
        }
        public async Task<IEnumerable<ProductCOADto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spProductCOA_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<ProductCOA>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductCOADto>>(Banks);
        }
        public async Task<IEnumerable<ProductCOADto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetProductCOAsByCategory";
            var parameters = new { CategoryId = categoryId };
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOA>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
        }
        public async Task<IEnumerable<ProductCOADto>> BulkLoadProductCOAsAsync(string procedure, object parameters = null)
        {
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOADto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
        }
        public async Task<IEnumerable<ProductCOADto>> GetPaginatedProductCOAsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "ProductCOA_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var ProductCOAs = await _dbConnection.QueryAsync<ProductCOADto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<ProductCOADto>>(ProductCOAs);
        }

        #endregion

        #region Add Method
        public async Task<int> AddAsync(ProductCOADto ProductCOADto)
        {
            var procedure = "spProductCOA_Insert";
            var entity = _mapper.Map<ProductCOA>(ProductCOADto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }
        public async Task<int> BulkInsertProductCOAsAsync(IEnumerable<ProductCOADto> ProductCOADtos)
        {
            var procedure = "ProductCOA_BulkInsert";
            var ProductCOAs = _mapper.Map<IEnumerable<ProductCOA>>(ProductCOADtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = ProductCOAs }, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region Update Method


        // Update an existing ProductCOA
        public async Task<int> UpdateAsync(ProductCOADto ProductCOADto)
        {
            try
            {
                var procedure = "spProductCOA_Update";
                var entity = _mapper.Map<ProductCOA>(ProductCOADto);
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
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spProductCOA_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteSaleAccountByProductIdAsync(int id)
        {
            var procedure = "spProductCOA_DeleteSaleAccountByProductId";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteBuyAccountByProductIdAsync(int id)
        {
            var procedure = "spProductCOA_DeleteBuyAccountByProductId";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
