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
    public interface IPOVATBreakdownRepository
    {
        #region Get
        Task<POVATBreakdownDto> GetByIdAsync(int id);
        Task<IEnumerable<POVATBreakdownDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<POVATBreakdownDto>> GetAllAsync();
        Task<IEnumerable<POVATBreakdownDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<POVATBreakdownDto>> GetPaginatedPOVATBreakdownsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(POVATBreakdownDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(POVATBreakdownDto dto);
        Task<int> BulkAddPOVATBreakdownsAsync(IEnumerable<POVATBreakdownDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(POVATBreakdownDto productDto);
        Task<IEnumerable<POVATBreakdownDto>> BulkLoadPOVATBreakdownsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(POVATBreakdownDto dto);
        Task<int> DeleteByPOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<POVATBreakdownDto>> GetAllByPOIdAsync(int SOId);
        Task<int> DeleteByPODtlIdAsync(int SODtlId);
        #endregion
    }

}
