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
    public interface IPODtlRepository //: IRepository<PODtl>
    {
        #region Get
        Task<PODtlDto> GetByIdAsync(int id);
        Task<IEnumerable<PODtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PODtlDto>> GetAllAsync();
        Task<IEnumerable<PODtlDto>> GetAllByPOIdAsync(int POId);
        Task<IEnumerable<PODtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PODtlDto>> GetPaginatedPODtlsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PODtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PODtlDto dto);
        Task<int> BulkAddPODtlsAsync(IEnumerable<PODtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PODtlDto productDto);
        Task<IEnumerable<PODtlDto>> BulkLoadPODtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PODtlDto dto);
        Task<int> DeleteByPOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        #endregion
    }

}
