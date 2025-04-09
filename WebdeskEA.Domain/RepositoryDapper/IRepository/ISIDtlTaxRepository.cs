using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ISIDtlTaxRepository //: IRepository<SIDtlTax>
    {
        #region Get
        Task<SIDtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<SIDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SIDtlTaxDto>> GetAllAsync();
        Task<IEnumerable<SIDtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SIDtlTaxDto>> GetPaginatedSIDtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SIDtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SIDtlTaxDto dto);
        Task<int> BulkAddSIDtlTaxsAsync(IEnumerable<SIDtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SIDtlTaxDto productDto);
        Task<IEnumerable<SIDtlTaxDto>> BulkLoadSIDtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SIDtlTaxDto dto);
        Task<int> DeleteBySIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SIDtlTaxDto>> GetAllBySIIdAsync(int SOId);
        Task<int> DeleteBySIDtlIdAsync(int SODtlId);
        #endregion
    }

}
