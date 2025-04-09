using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IProductCOARepository
    {
        Task<ProductCOADto> GetByIdAsync(int id);
        Task<IEnumerable<ProductCOADto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<ProductCOADto>> GetAllAsync();
        Task<int> AddAsync(ProductCOADto dto);
        Task<int> UpdateAsync(ProductCOADto dto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<ProductCOADto>> GetByCategoryAsync(int categoryId);
        Task<int> BulkInsertProductCOAsAsync(IEnumerable<ProductCOADto> Dtos);
        Task<IEnumerable<ProductCOADto>> BulkLoadProductCOAsAsync(string procedure, object parameters = null);
        Task<IEnumerable<ProductCOADto>> GetPaginatedProductCOAsAsync(int pageIndex, int pageSize, string filter);
        Task<IEnumerable<int>> GetSelectedSaleCOAsByProductIdAsync(int ProductId);
        Task<int> DeleteSaleAccountByProductIdAsync(int id);
        Task<int> DeleteBuyAccountByProductIdAsync(int id);
        Task<IEnumerable<int>> GetSelectedBuyCOAsByProductIdAsync(int ProductId);
    }

}
