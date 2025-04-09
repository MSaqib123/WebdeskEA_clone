using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IRolePermissionRepository
    {
        Task<RolePermissionDto> GetByIdAsync(int id);
        Task<IEnumerable<RolePermissionDto>> GetAllAsync();
        Task<IEnumerable<RolePermissionDto>> GetAllByRoleIdAsync(int RoleId);

        Task<int> AddAsync(RolePermissionDto rolePermissionDto);
        Task<int> BulkAddAsync(RolePermissionDto rolePermissionDto);

        Task<int> UpdateAsync(RolePermissionDto rolePermissionDto);
        Task<int> DeleteAsync(int id);
        Task<int> DeleteByRoleIdAsync(int RoleId);
    }

}
