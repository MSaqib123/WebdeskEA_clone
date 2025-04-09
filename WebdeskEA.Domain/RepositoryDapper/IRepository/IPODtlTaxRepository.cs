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
    public interface IPODtlTaxRepository //: IRepository<PODtlTax>
    {
        #region Get
        Task<PODtlTaxDto> GetByIdAsync(int id);
        Task<IEnumerable<PODtlTaxDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<PODtlTaxDto>> GetAllAsync();
        Task<IEnumerable<PODtlTaxDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<PODtlTaxDto>> GetPaginatedPODtlTaxsAsync(int pageIndex, int pageSize, string filter);

        #endregion

        #region Add
        Task<int> AddAsync(PODtlTaxDto productDto, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<int> AddTransactionAsync(PODtlTaxDto dto);
        Task<int> BulkAddPODtlTaxsAsync(IEnumerable<PODtlTaxDto> productDtos);

        #endregion

        #region Update
        Task<int> UpdateAsync(PODtlTaxDto productDto);
        Task<IEnumerable<PODtlTaxDto>> BulkLoadPODtlTaxsAsync(string procedure, object parameters = null);

        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateTransactionAsync(PODtlTaxDto dto);
        Task<int> DeleteByPOIdAsync(int SOId, IDbConnection connection = null, IDbTransaction transaction = null);
        Task<IEnumerable<PODtlTaxDto>> GetAllByPOIdAsync(int SOId);
        Task<int> DeleteByPODtlIdAsync(int SODtlId);
        #endregion
    }

}
