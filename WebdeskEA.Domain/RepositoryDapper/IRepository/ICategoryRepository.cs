using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICategoryRepository
    {
        Task<CategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<int> AddAsync(CategoryDto CategoryDto);
        Task<int> UpdateAsync(CategoryDto CategoryDto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<CategoryDto>> GetByCategoryAsync(int categoryId);
        Task<int> BulkInsertCategorysAsync(IEnumerable<CategoryDto> CategoryDtos);
        Task<IEnumerable<CategoryDto>> BulkLoadCategorysAsync(string procedure, object parameters = null);
        Task<IEnumerable<CategoryDto>> GetPaginatedCategorysAsync(int pageIndex, int pageSize, string filter);
    }

}
