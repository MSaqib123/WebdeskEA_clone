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
    public interface ISIDtlRepository //: IRepository<SIDtl>
    {
        #region Get
        Task<SIDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<SIDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SIDtlDto>> GetAllAsync();
        Task<IEnumerable<SIDtlDto>> GetAllBySIIdAsync(int SOId);
        Task<IEnumerable<SIDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SIDtlDto>> GetPaginatedSIDtlsAsync(int pageIndex, int pageSize, string filter);
        #endregion

        #region Add
        Task<int> AddAsync(SIDtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SIDtlDto dto);
        Task<int> BulkAddSIDtlsAsync(IEnumerable<SIDtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SIDtlDto productDto);
        Task<IEnumerable<SIDtlDto>> BulkLoadSIDtlsAsync(string procedure, object parameters = null);
        Task<int> UpdateTransactionAsync(SIDtlDto dto);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> DeleteBySIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        #endregion
    }

}
