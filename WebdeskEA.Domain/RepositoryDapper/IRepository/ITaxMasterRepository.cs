using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ITaxMasterRepository
    {
        Task<TaxMasterDto> GetByIdAsync(int id);
        Task<IEnumerable<TaxMasterDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<TaxMasterDto>> GetAllAsync();
        Task<int> AddAsync(TaxMasterDto TaxMasterDto);
        Task<int> UpdateAsync(TaxMasterDto TaxMasterDto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<TaxMasterDto>> GetByCategoryAsync(int categoryId);
        Task<int> BulkInsertTaxMastersAsync(IEnumerable<TaxMasterDto> TaxMasterDtos);
        Task<IEnumerable<TaxMasterDto>> BulkLoadTaxMastersAsync(string procedure, object parameters = null);
        Task<IEnumerable<TaxMasterDto>> GetPaginatedTaxMastersAsync(int pageIndex, int pageSize, string filter);
        Task<TaxMasterDto> GetByIdTenantId(int id, int TenantId);
    }

}
