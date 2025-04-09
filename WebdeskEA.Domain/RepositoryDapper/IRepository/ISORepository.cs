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
    public interface ISORepository //: IRepository<SO>
    {
        #region Get
        Task<SODto> GetByIdAsync(int id);
        Task<IEnumerable<SODto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SODto>> GetAllAsync();
        Task<IEnumerable<SODto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView typeView = TypeView.Create, int id = 0);
        Task<IEnumerable<SODto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SODto>> GetPaginatedSOsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SODto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SODto dto);
        Task<int> BulkAddSOsAsync(IEnumerable<SODto> productDtos);
        #endregion

        #region Update
        Task<int> UpdateAsync(SODto productDto, IDbTransaction transaction = null);
        Task<int> UpdateTransactionAsync(SODto dto);
        Task<IEnumerable<SODto>> BulkLoadSOsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        #endregion

        #region SI_proccess
        Task<bool> GenerateSIBySO(SODto dto);
        Task<bool> DeleteSIBySO(int SOId);
        Task<IEnumerable<SODto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        Task<(bool IsSuccess, string Message)> ForceDeleteSOAsync(int soId);
        Task<IEnumerable<SODto>> GetAllHoldPOSByTenantCompanyAsync(int TenantId, int CompanyId);
        #endregion
    }

}
