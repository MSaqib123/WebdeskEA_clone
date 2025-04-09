using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IProductRepository //: IRepository<Product>
    {
        #region Get
        Task<ProductDto> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetPaginatedProductsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(ProductDto productDto);
        Task<int> AddTransactionAsync(ProductDto dto);
        Task<int> BulkAddProductsAsync(IEnumerable<ProductDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(ProductDto productDto);
        Task<IEnumerable<ProductDto>> BulkLoadProductsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(ProductDto dto);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAsync(int TenantId, int CompanyId, int FinancialYears);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAndIsPurchaseAsync(int TenantId, int CompanyId, int FinancialYears);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyFinancialYearsIdAndIsSaleAsync(int TenantId, int CompanyId, int FinancialYears);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAndIsPurchaseAsync(int TenantId, int CompanyId);
        Task<IEnumerable<ProductDto>> GetAllByTenantCompanyIdAndIsSaleAsync(int TenantId, int CompanyId);
        #endregion
    }

}
