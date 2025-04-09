using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICOARepository
    {
        Task<COADto> GetByIdAsync(int id, int TenantId, int CompanyId);
        Task<IEnumerable<COADto>> GetAllAsync();
        Task<int> AddAsync(COADto Dto);
        Task<int> UpdateAsync(COADto Dto);
        Task<int> DeletesAsync(int id);
        Task<IEnumerable<COADto>> GetByNameAsync(string name);
        Task<int> BulkAddProcAsync(IEnumerable<COADto> Dtos);
        Task<IEnumerable<COADto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<COADto>> GetAllByCompanyIdOrAccountTypeAsync(int CompanyId, string AccountType = "");
        Task<IEnumerable<COADto>> GetAllByCompanyIdOrAccountTypeIdAsync(int TenantId, int CompanyId, int COATypeId);
    }
}
