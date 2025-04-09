using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IRoleRepository
    {
        Task<RoleDto> GetByIdAsync(int id);
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<string> GetAllRolesAsStringAsync();
        Task<int> AddAsync(RoleDto roleDto);
        Task<int> UpdateAsync(RoleDto roleDto);
        Task<int> DeleteAsync(int id);
    }

}
