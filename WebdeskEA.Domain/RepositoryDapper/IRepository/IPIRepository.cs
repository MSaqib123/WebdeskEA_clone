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
    public interface IPIRepository //: IRepository<PO>
    {
        #region Get
        Task<PIDto> GetByIdAsync(int id);
        Task<IEnumerable<PIDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PIDto>> GetAllAsync();
        Task<IEnumerable<PIDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PIDto>> GetPaginatedPIsAsync(int pageIndex, int pageSize, string filter);
        Task<PIDto> GetByPOIdAsync(int id);
        Task<IEnumerable<PIDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);

        #endregion

        #region Add
        Task<int> AddAsync(PIDto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PIDto dto);
        Task<int> BulkAddPIsAsync(IEnumerable<PIDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PIDto productDto, IDbTransaction transaction = null);
        Task<IEnumerable<PIDto>> BulkLoadPIsAsync(string procedure, object parameters = null);
        Task<int> UpdateTransactionAsync(PIDto dto);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<PIDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0);
        #endregion


        #region PRProssess
        Task<bool> DeletePRByPI(int PIId);
        Task<bool> GeneratePRByPI(PIDto dto);
        Task<(bool IsSuccess, string Message)> ForceDeletePIAsync(int piId);
        #endregion
    }

}
