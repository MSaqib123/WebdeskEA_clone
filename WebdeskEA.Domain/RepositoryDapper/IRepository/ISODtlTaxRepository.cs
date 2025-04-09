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
    public interface ISODtlTaxRepository //: IRepository<SODtlTax>
    {
        #region Get
        Task<SODtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<SODtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SODtlTaxDto>> GetAllAsync();
        Task<IEnumerable<SODtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SODtlTaxDto>> GetPaginatedSODtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SODtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SODtlTaxDto dto);
        Task<int> BulkAddSODtlTaxsAsync(IEnumerable<SODtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SODtlTaxDto productDto);
        Task<IEnumerable<SODtlTaxDto>> BulkLoadSODtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SODtlTaxDto dto);
        Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SODtlTaxDto>> GetAllBySOIdAsync(int SOId);
        Task<int> DeleteBySODtlIdAsync(int SODtlId);
        #endregion
    }

}
