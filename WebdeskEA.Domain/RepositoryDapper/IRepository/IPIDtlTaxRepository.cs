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
    public interface IPIDtlTaxRepository //: IRepository<PIDtlTax>
    {
        #region Get
        Task<PIDtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<PIDtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PIDtlTaxDto>> GetAllAsync();
        Task<IEnumerable<PIDtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PIDtlTaxDto>> GetPaginatedPIDtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PIDtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PIDtlTaxDto dto);
        Task<int> BulkAddPIDtlTaxsAsync(IEnumerable<PIDtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PIDtlTaxDto productDto);
        Task<IEnumerable<PIDtlTaxDto>> BulkLoadPIDtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PIDtlTaxDto dto);
        Task<int> DeleteByPIIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PIDtlTaxDto>> GetAllByPIIdAsync(int SOId);
        Task<int> DeleteByPIDtlIdAsync(int SODtlId);
        #endregion
    }

}
