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
    public interface IPRRepository //: IRepository<PO>
    {
        #region Get
        Task<PRDto> GetByIdAsync(int id);
        Task<IEnumerable<PRDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PRDto>> GetAllAsync();
        Task<IEnumerable<PRDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PRDto>> GetPaginatedPRsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PRDto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PRDto dto);
        Task<int> BulkAddPRsAsync(IEnumerable<PRDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PRDto productDto, IDbTransaction transaction = null);
        Task<IEnumerable<PRDto>> BulkLoadPRsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PRDto dto);
        Task<PRDto> GetByPIIdAsync(int id);
        Task<IEnumerable<PRDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        Task<(bool IsSuccess, string Message)> ForceDeletePRAsync(int prId);
        #endregion
    }

}
