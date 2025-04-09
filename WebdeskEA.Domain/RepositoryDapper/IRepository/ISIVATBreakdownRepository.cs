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
    public interface ISIVATBreakdownRepository
    {
        #region Get
        Task<SIVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<SIVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SIVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<SIVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SIVATBreakdownDto>> GetPaginatedSIVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SIVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SIVATBreakdownDto dto);
        Task<int> BulkAddSIVATBreakdownsAsync(IEnumerable<SIVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SIVATBreakdownDto productDto);
        Task<IEnumerable<SIVATBreakdownDto>> BulkLoadSIVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SIVATBreakdownDto dto);
        Task<int> DeleteBySIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SIVATBreakdownDto>> GetAllBySIIdAsync(int SOId);
        Task<int> DeleteBySIDtlIdAsync(int SODtlId);
        #endregion
    }

}
