using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IOBDtlRepository //: IRepository<OBDtl>
    {
        #region Get
        Task<OBDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<OBDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<OBDtlDto>> GetAllAsync();
        Task<IEnumerable<OBDtlDto>> GetAllByOBIdAsync(int OBId);
        Task<IEnumerable<OBDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<OBDtlDto>> GetPaginatedOBDtlsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(OBDtlDto dto);
        Task<int> AddTransactionAsync(OBDtlDto dto);
        Task<int> BulkAddOBDtlsAsync(IEnumerable<OBDtlDto> dtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(OBDtlDto productDto);
        Task<IEnumerable<OBDtlDto>> BulkLoadOBDtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(OBDtlDto dto);
        Task<int> DeleteByOBIdAsync(int SOId);
        #endregion
    }

}
