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
    public interface ISODtlRepository //: IRepository<SODtl>
    {
        #region Get
        Task<SODtlDto> GetByIdAsync(int id);
        Task<IEnumerable<SODtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SODtlDto>> GetAllAsync();
        Task<IEnumerable<SODtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SODtlDto>> GetPaginatedSODtlsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(SODtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SODtlDto dto);
        Task<int> BulkAddSODtlsAsync(IEnumerable<SODtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SODtlDto productDto);
        Task<IEnumerable<SODtlDto>> BulkLoadSODtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(SODtlDto dto);
        Task<int> DeleteBySOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<SODtlDto>> GetAllBySOIdAsync(int SOId);
        #endregion
    }

}
