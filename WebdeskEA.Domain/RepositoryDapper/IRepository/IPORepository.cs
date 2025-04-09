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
    public interface IPORepository //: IRepository<PO>
    {
        #region Get
        Task<PODto> GetByIdAsync(int id);
        Task<IEnumerable<PODto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PODto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0);
        Task<IEnumerable<PODto>> GetAllAsync();
        Task<IEnumerable<PODto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PODto>> GetPaginatedPOsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PODto productDto, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PODto dto);
        Task<int> BulkAddPOsAsync(IEnumerable<PODto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PODto productDto, IDbTransaction transaction = null);
        Task<int> UpdateTransactionAsync(PODto dto);
        Task<IEnumerable<PODto>> BulkLoadPOsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        #endregion


        #region Delete_Proccess
        Task<(bool IsSuccess, string Message)> ForceDeletePOAsync(int poId);
        #endregion


        #region SI_proccess
        Task<bool> GeneratePIByPO(PODto dto);
        Task<bool> DeletePIByPO(int SOId);
        Task<IEnumerable<PODto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        #endregion
    }

}
