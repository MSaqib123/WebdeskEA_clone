using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.MappingModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryEntity.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUserDto>
    {
        Task<ApplicationUserDto> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();
        Task<IdentityResult> AddAsync(ApplicationUserDto model);
        Task<IdentityResult> UpdateAsync(ApplicationUserDto model);
        Task<IdentityResult> DeleteAsync(string id);
        Task<IEnumerable<ApplicationUserDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
    }
}
