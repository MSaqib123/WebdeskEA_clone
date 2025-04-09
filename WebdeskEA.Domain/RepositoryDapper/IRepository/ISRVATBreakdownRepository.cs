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
    public interface ISRVATBreakdownRepository
    {
        #region Get
        Task<SRVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<SRVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SRVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<SRVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SRVATBreakdownDto>> GetPaginatedSRVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SRVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SRVATBreakdownDto dto);
        Task<int> BulkAddSRVATBreakdownsAsync(IEnumerable<SRVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SRVATBreakdownDto productDto);
        Task<IEnumerable<SRVATBreakdownDto>> BulkLoadSRVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SRVATBreakdownDto dto);
        Task<int> DeleteBySRIdAsync(int SRId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SRVATBreakdownDto>> GetAllBySRIdAsync(int SRId);
        Task<int> DeleteBySRDtlIdAsync(int SRDtlId);
        #endregion
    }

}
