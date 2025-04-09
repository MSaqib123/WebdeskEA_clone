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
    public interface IPIVATBreakdownRepository
    {
        #region Get
        Task<PIVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<PIVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PIVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<PIVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PIVATBreakdownDto>> GetPaginatedPIVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PIVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PIVATBreakdownDto dto);
        Task<int> BulkAddPIVATBreakdownsAsync(IEnumerable<PIVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PIVATBreakdownDto productDto);
        Task<IEnumerable<PIVATBreakdownDto>> BulkLoadPIVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PIVATBreakdownDto dto);
        Task<int> DeleteByPIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PIVATBreakdownDto>> GetAllByPIIdAsync(int SOId);
        Task<int> DeleteByPIDtlIdAsync(int SODtlId);
        #endregion
    }

}
