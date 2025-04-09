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

    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly IMapper _mapper;
        public BrandRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        // Get Brand by Id
        public async Task<BrandDto> GetByIdAsync(int id)
        {
            var procedure = "spBrand_GetById";
            var parameters = new { Id = id };
            var Brand = await _dbConnection.QueryFirstOrDefaultAsync<BrandDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<BrandDto>(Brand);
        }

        // Get all Brands
        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var procedure = "spBrand_GetAll";
            var Brands = await _dbConnection.QueryAsync<Brand>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BrandDto>>(Brands);
        }

        public async Task<IEnumerable<BrandDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spBrand_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, ParentCompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Brand>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BrandDto>>(Banks);
        }

        // Get Brands by category
        public async Task<IEnumerable<BrandDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetBrandsByCategory";
            var parameters = new { CategoryId = categoryId };
            var Brands = await _dbConnection.QueryAsync<Brand>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BrandDto>>(Brands);
        }

        // Bulk load Brands
        public async Task<IEnumerable<BrandDto>> BulkLoadBrandsAsync(string procedure, object parameters = null)
        {
            var Brands = await _dbConnection.QueryAsync<BrandDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BrandDto>>(Brands);
        }



        // Get paginated Brands
        public async Task<IEnumerable<BrandDto>> GetPaginatedBrandsAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "Brand_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var Brands = await _dbConnection.QueryAsync<BrandDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<BrandDto>>(Brands);
        }

        #endregion

        #region Add Method

        // Add a new Brand
        public async Task<int> AddAsync(BrandDto BrandDto)
        {
            var procedure = "spBrand_Insert";
            var entity = _mapper.Map<Brand>(BrandDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        // Bulk insert Brands
        public async Task<int> BulkInsertBrandsAsync(IEnumerable<BrandDto> BrandDtos)
        {
            var procedure = "Brand_BulkInsert";
            var Brands = _mapper.Map<IEnumerable<Brand>>(BrandDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = Brands }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing Brand
        public async Task<int> UpdateAsync(BrandDto BrandDto)
        {
            try
            {
                var procedure = "spBrand_Update";
                var entity = _mapper.Map<Brand>(BrandDto);
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
        // Delete a Brand by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spBrand_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
