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
    public interface IPRDtlTaxRepository //: IRepository<PRDtlTax>
    {
        #region Get
        Task<PRDtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<PRDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PRDtlTaxDto>> GetAllAsync();
        Task<IEnumerable<PRDtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PRDtlTaxDto>> GetPaginatedPRDtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PRDtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PRDtlTaxDto dto);
        Task<int> BulkAddPRDtlTaxsAsync(IEnumerable<PRDtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PRDtlTaxDto productDto);
        Task<IEnumerable<PRDtlTaxDto>> BulkLoadPRDtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PRDtlTaxDto dto);
        Task<int> DeleteByPRIdAsync(int PRId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PRDtlTaxDto>> GetAllByPRIdAsync(int PRId);
        Task<int> DeleteByPRDtlIdAsync(int PRDtlId);
        #endregion
    }

}
