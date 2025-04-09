using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IBrandRepository
    {
        Task<BrandDto> GetByIdAsync(int id);
        Task<IEnumerable<BrandDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<BrandDto>> GetAllAsync();
        Task<int> AddAsync(BrandDto BrandDto);
        Task<int> UpdateAsync(BrandDto BrandDto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<BrandDto>> GetByCategoryAsync(int categoryId);
        Task<int> BulkInsertBrandsAsync(IEnumerable<BrandDto> BrandDtos);
        Task<IEnumerable<BrandDto>> BulkLoadBrandsAsync(string procedure, object parameters = null);
        Task<IEnumerable<BrandDto>> GetPaginatedBrandsAsync(int pageIndex, int pageSize, string filter);
    }

}
