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
    public interface IPRVATBreakdownRepository
    {
        #region Get
        Task<PRVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<PRVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PRVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<PRVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PRVATBreakdownDto>> GetPaginatedPRVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PRVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PRVATBreakdownDto dto);
        Task<int> BulkAddPRVATBreakdownsAsync(IEnumerable<PRVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PRVATBreakdownDto productDto);
        Task<IEnumerable<PRVATBreakdownDto>> BulkLoadPRVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PRVATBreakdownDto dto);
        Task<int> DeleteByPRIdAsync(int PRId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PRVATBreakdownDto>> GetAllByPRIdAsync(int PRId);
        Task<int> DeleteByPRDtlIdAsync(int PRDtlId);
        #endregion
    }

}
