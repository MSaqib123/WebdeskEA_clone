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
    public interface IPIDtlRepository //: IRepository<PIDtl>
    {
        #region Get
        Task<PIDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<PIDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PIDtlDto>> GetAllAsync();
        Task<IEnumerable<PIDtlDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PIDtlDto>> GetPaginatedPIDtlsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PIDtlDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PIDtlDto dto);
        Task<int> BulkAddPIDtlsAsync(IEnumerable<PIDtlDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PIDtlDto productDto);
        Task<IEnumerable<PIDtlDto>> BulkLoadPIDtlsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PIDtlDto dto);
        Task<int> DeleteByPIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PIDtlDto>> GetAllByPIIdAsync(int PIId);
        #endregion
    }

}
