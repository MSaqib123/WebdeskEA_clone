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
    public interface IOBRepository //: IRepository<OB>
    {
        #region Get
        Task<OBDto> GetByIdAsync(int id);
        Task<IEnumerable<OBDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<OBDto>> GetAllNotInUsedByTenantCompanyIdAsync(int TenantId, int CompanyId, TypeView type = TypeView.Create, int id = 0);
        Task<IEnumerable<OBDto>> GetAllAsync();
        Task<IEnumerable<OBDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<OBDto>> GetPaginatedOBsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(OBDto productDto);
        Task<int> AddTransactionAsync(OBDto dto);
        Task<int> BulkAddOBsAsync(IEnumerable<OBDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(OBDto productDto);
        Task<int> UpdateTransactionAsync(OBDto dto);
        Task<IEnumerable<OBDto>> BulkLoadOBsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        #endregion

        #region SI_proccess
        Task<bool> GeneratePIByOB(OBDto dto);
        Task<bool> DeletePIByOB(int SOId);
        Task<IEnumerable<OBDto>> GetAllByTenantCompanyFinancialYearIdAsync(int TenantId, int CompanyId, int FinancialYearId);
        #endregion
    }

}
