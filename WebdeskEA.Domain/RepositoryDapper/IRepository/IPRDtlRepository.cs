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
    public interface IPRDtlRepository //: IRepository<PRDtl>
    {
        #region Get
        Task<PRDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<PRDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PRDtlDto>> GetAllAsync();
        Task<IEnumerable<PRDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PRDtlDto>> GetPaginatedPRDtlsAsync(int pageIndex, int pageSize, string filter);
        #endregion

        #region Add
        Task<int> AddAsync(PRDtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PRDtlDto dto);
        Task<int> BulkAddPRDtlsAsync(IEnumerable<PRDtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PRDtlDto productDto);
        Task<IEnumerable<PRDtlDto>> BulkLoadPRDtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PRDtlDto dto);
        Task<int> DeleteByPRIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PRDtlDto>> GetAllByPRIdAsync(int PRId);
        #endregion
    }

}
