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

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly IMapper _mapper;
        public CategoryRepository(IDapperDbConnectionFactory dbConnectionFactory, IMapper mapper)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
        }

        #region Get Methods

        // Get Category by Id
        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var procedure = "spCategory_GetById";
            var parameters = new { Id = id };
            var Category = await _dbConnection.QueryFirstOrDefaultAsync<CategoryDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<CategoryDto>(Category);
        }

        // Get all Categorys
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var procedure = "spCategory_GetAll";
            var Categorys = await _dbConnection.QueryAsync<Category>(procedure, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CategoryDto>>(Categorys);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId)
        {
            var procedure = "spCategory_GetAllByTenantAndCompanyId";
            var parameters = new { TenantId = TenantId, CompanyId = CompanyId };
            var Banks = await _dbConnection.QueryAsync<Category>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CategoryDto>>(Banks);
        }

        // Get Categorys by category
        public async Task<IEnumerable<CategoryDto>> GetByCategoryAsync(int categoryId)
        {
            var procedure = "GetCategorysByCategory";
            var parameters = new { CategoryId = categoryId };
            var Categorys = await _dbConnection.QueryAsync<Category>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CategoryDto>>(Categorys);
        }

        // Bulk load Categorys
        public async Task<IEnumerable<CategoryDto>> BulkLoadCategorysAsync(string procedure, object parameters = null)
        {
            var Categorys = await _dbConnection.QueryAsync<CategoryDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CategoryDto>>(Categorys);
        }



        // Get paginated Categorys
        public async Task<IEnumerable<CategoryDto>> GetPaginatedCategorysAsync(int pageIndex, int pageSize, string filter)
        {
            var procedure = "Category_GetPaginated";
            var parameters = new { PageIndex = pageIndex, PageSize = pageSize, Filter = filter };
            var Categorys = await _dbConnection.QueryAsync<CategoryDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            return _mapper.Map<IEnumerable<CategoryDto>>(Categorys);
        }

        #endregion

        #region Add Method

        // Add a new Category
        public async Task<int> AddAsync(CategoryDto CategoryDto)
        {
            var procedure = "spCategory_Insert";
            var entity = _mapper.Map<Category>(CategoryDto);
            var parameters = CommonDapperMethod.GenerateParameters(entity);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Id");
        }


        // Bulk insert Categorys
        public async Task<int> BulkInsertCategorysAsync(IEnumerable<CategoryDto> CategoryDtos)
        {
            var procedure = "Category_BulkInsert";
            var Categorys = _mapper.Map<IEnumerable<Category>>(CategoryDtos);
            return await _dbConnection.ExecuteAsync(procedure, new { Items = Categorys }, commandType: CommandType.StoredProcedure);
        }


        #endregion

        #region Update Method


        // Update an existing Category
        public async Task<int> UpdateAsync(CategoryDto CategoryDto)
        {
            try
            {
                var procedure = "spCategory_Update";
                var entity = _mapper.Map<Category>(CategoryDto);
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
        // Delete a Category by Id
        public async Task<int> DeleteAsync(int id)
        {
            var procedure = "spCategory_Delete";
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

    }
}
