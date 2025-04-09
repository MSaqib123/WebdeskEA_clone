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
    public interface ISRDtlRepository //: IRepository<SRDtl>
    {
        #region Get
        Task<SRDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<SRDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<SRDtlDto>> GetAllAsync();
        Task<IEnumerable<SRDtlDto>> GetAllBySRIdAsync(int SOId);
        Task<IEnumerable<SRDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<SRDtlDto>> GetPaginatedSRDtlsAsync(int pageIndex, int pageSize, string filter);
        #endregion

        #region Add
        Task<int> AddAsync(SRDtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(SRDtlDto dto);
        Task<int> BulkAddSRDtlsAsync(IEnumerable<SRDtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(SRDtlDto productDto);
        Task<IEnumerable<SRDtlDto>> BulkLoadSRDtlsAsync(string procedure, object parameters = null);
        Task<int> UpdateTransactionAsync(SRDtlDto dto);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> DeleteBySRIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        #endregion
    }

}
