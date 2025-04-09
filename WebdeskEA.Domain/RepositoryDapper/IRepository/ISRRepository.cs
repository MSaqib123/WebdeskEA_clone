using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using System.Data;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ISRRepository //: IRepository<SR>
    {
        #region Get
        Task<SRDto> GetByIdAsync(int id);
        Task<IEnumerable<SRDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SRDto>> GetAllAsync();
        Task<SRDto> GetBySIIdAsync(int id);
        Task<IEnumerable<SRDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SRDto>> GetPaginatedSRsAsync(int pageIndex, int pageSize, string filter);
        Task<IEnumerable<SRDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);


        #endregion

        #region Add
        Task<int> AddAsync(SRDto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SRDto dto);
        Task<int> BulkAddSRsAsync(IEnumerable<SRDto> productDtos);
        #endregion

        #region Update
        Task<int> UpdateAsync(SRDto productDto, IDbTransaction transaction = null);
        Task<int> UpdateTransactionAsync(SRDto dto);
        Task<IEnumerable<SRDto>> BulkLoadSRsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<(bool IsSuccess, string Message)> ForceDeleteSRAsync(int srId);
        #endregion

    }

}
