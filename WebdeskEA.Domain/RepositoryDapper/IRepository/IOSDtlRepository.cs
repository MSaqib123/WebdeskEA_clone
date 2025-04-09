using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IOSDtlRepository //: IRepository<OSDtl>
    {
        #region Get
        Task<OSDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<OSDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<OSDtlDto>> GetAllAsync();
        Task<IEnumerable<OSDtlDto>> GetAllByOSIdAsync(int OSId);
        Task<IEnumerable<OSDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<OSDtlDto>> GetPaginatedOSDtlsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(OSDtlDto dto);
        Task<int> AddTransactionAsync(OSDtlDto dto);
        Task<int> BulkAddOSDtlsAsync(IEnumerable<OSDtlDto> dtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(OSDtlDto productDto);
        Task<IEnumerable<OSDtlDto>> BulkLoadOSDtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(OSDtlDto dto);
        Task<int> DeleteByOSIdAsync(int SOId);
        #endregion
    }

}
