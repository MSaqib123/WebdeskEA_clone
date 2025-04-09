using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using System.Data;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ISIRepository //: IRepository<SI>
    {
        #region Get
        Task<SIDto> GetByIdAsync(int id);
        Task<IEnumerable<SIDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SIDto>> GetAllAsync();
        Task<IEnumerable<SIDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SIDto>> GetPaginatedSIsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SIDto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SIDto dto);
        Task<int> BulkAddSIsAsync(IEnumerable<SIDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SIDto productDto, IDbTransaction transaction = null);
        Task<IEnumerable<SIDto>> BulkLoadSIsAsync(string procedure, object parameters = null);
        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SIDto dto);
        Task<SIDto> GetBySOIdAsync(int id);
        Task<IEnumerable<SIDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        Task<IEnumerable<SIDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView typeView = TypeView.Create, int id = 0);
        #endregion

        #region SR_proccess
        Task<bool> GenerateSRBySI(SIDto dto);
        Task<bool> DeleteSRBySI(int SIId);
        Task<(bool IsSuccess, string Message)> ForceDeleteSIAsync(int soId);
        #endregion
    }

}
