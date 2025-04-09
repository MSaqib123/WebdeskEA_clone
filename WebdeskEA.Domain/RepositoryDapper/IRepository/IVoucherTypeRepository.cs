using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IVoucherTypeRepository
    {
        Task<VoucherTypeDto> GetByIdAsync(int id, int TenantId, int CompanyId);
        Task<IEnumerable<VoucherTypeDto>> GetAllAsync();
        Task<int> AddAsync(VoucherTypeDto Dto);
        Task<int> UpdateAsync(VoucherTypeDto Dto);
        Task<int> DeletesAsync(int id);
        Task<IEnumerable<VoucherTypeDto>> GetByNameAsync(string name);
        Task<int> BulkAddProcAsync(IEnumerable<VoucherTypeDto> Dtos);
        Task<IEnumerable<VoucherTypeDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<VoucherTypeDto>> GetAllByCompanyIdOrAccountTypeAsync(int CompanyId, string AccountType = "");
        Task<IEnumerable<VoucherTypeDto>> GetAllByCompanyIdOrAccountTypeIdAsync(int TenantId, int CompanyId, int VoucherTypeTypeId);
    }
}
