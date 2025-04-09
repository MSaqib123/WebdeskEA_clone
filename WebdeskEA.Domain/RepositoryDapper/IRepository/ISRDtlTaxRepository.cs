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
    public interface ISRDtlTaxRepository //: IRepository<SRDtlTax>
    {
        #region Get
        Task<SRDtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<SRDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SRDtlTaxDto>> GetAllAsync();
        Task<IEnumerable<SRDtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SRDtlTaxDto>> GetPaginatedSRDtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SRDtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SRDtlTaxDto dto);
        Task<int> BulkAddSRDtlTaxsAsync(IEnumerable<SRDtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SRDtlTaxDto productDto);
        Task<IEnumerable<SRDtlTaxDto>> BulkLoadSRDtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SRDtlTaxDto dto);
        Task<int> DeleteBySRIdAsync(int SRId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SRDtlTaxDto>> GetAllBySRIdAsync(int SRId);
        Task<int> DeleteBySRDtlIdAsync(int SRDtlId);
        #endregion
    }

}
