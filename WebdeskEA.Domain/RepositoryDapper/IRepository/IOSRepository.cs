using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IOSRepository //: IRepository<OS>
    {
        #region Get
        Task<OSDto> GetByIdAsync(int id);
        Task<IEnumerable<OSDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<OSDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0);
        Task<IEnumerable<OSDto>> GetAllAsync();
        Task<IEnumerable<OSDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<OSDto>> GetPaginatedOSsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(OSDto productDto);
        Task<int> AddTransactionAsync(OSDto dto);
        Task<int> BulkAddOSsAsync(IEnumerable<OSDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(OSDto productDto);
        Task<int> UpdateTransactionAsync(OSDto dto);
        Task<IEnumerable<OSDto>> BulkLoadOSsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        #endregion

        #region SI_proccess
        Task<bool> GeneratePIByOS(OSDto dto);
        Task<bool> DeletePIByOS(int SOId);
        Task<IEnumerable<OSDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        #endregion
    }

}
