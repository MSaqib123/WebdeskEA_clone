using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IPOSConfigRepository
    {
        Task<POSConfigDto> GetByIdAsync(int id);
        Task<IEnumerable<POSConfigDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<POSConfigDto>> GetAllAsync();
        Task<int> AddAsync(POSConfigDto POSConfigDto);
        Task<int> UpdateAsync(POSConfigDto POSConfigDto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<POSConfigDto>> GetByPOSConfigAsync(int POSConfigId);
        Task<int> BulkInsertPOSConfigsAsync(IEnumerable<POSConfigDto> POSConfigDtos);
        Task<IEnumerable<POSConfigDto>> BulkLoadPOSConfigsAsync(string procedure, object parameters = null);
        Task<IEnumerable<POSConfigDto>> GetPaginatedPOSConfigsAsync(int pageIndex, int pageSize, string filter);
        Task<POSConfigDto> GetByIdTenantIdCompanyIdAsync(int id, int TenantId, int CompanyId);
        Task<POSConfigDto> GetByActiveTenantIdCompanyIdAsync(int TenantId, int CompanyId);
    }

}
