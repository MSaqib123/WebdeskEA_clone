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
    public interface ISOVATBreakdownRepository
    {
        #region Get
        Task<SOVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<SOVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SOVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<SOVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SOVATBreakdownDto>> GetPaginatedSOVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SOVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SOVATBreakdownDto dto);
        Task<int> BulkAddSOVATBreakdownsAsync(IEnumerable<SOVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SOVATBreakdownDto productDto);
        Task<IEnumerable<SOVATBreakdownDto>> BulkLoadSOVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SOVATBreakdownDto dto);
        Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SOVATBreakdownDto>> GetAllBySOIdAsync(int SOId);
        Task<int> DeleteBySODtlIdAsync(int SODtlId);
        #endregion
    }

}
