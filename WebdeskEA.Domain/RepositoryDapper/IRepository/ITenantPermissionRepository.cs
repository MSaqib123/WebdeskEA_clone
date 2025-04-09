using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ITenantPermissionRepository
    {
        Task<TenantPermissionDto> GetByIdAsync(int id);
        Task<IEnumerable<TenantPermissionDto>> GetAllAsync();
        Task<int> AddAsync(TenantPermissionDto dto);
        Task<int> UpdateAsync(TenantPermissionDto dto);
        Task<int> DeleteAsync(int id);
        Task<int> DeleteByTenantIdAsync(int TenantId);
    }
}
