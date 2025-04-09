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
    public interface IVoucherRepository
    {
        Task<VoucherDto> GetByIdAsync(int id, int TenantId, int CompanyId);
        Task<IEnumerable<VoucherDto>> GetAllAsync();
        Task<int> AddAsync(VoucherDto Dto);
        Task<int> UpdateAsync(VoucherDto Dto);
        Task<int> DeletesAsync(int id);
        Task<IEnumerable<VoucherDto>> GetByNameAsync(string name);
        Task<int> BulkAddProcAsync(IEnumerable<VoucherDto> Dtos);
        Task<IEnumerable<VoucherDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<IEnumerable<VoucherDto>> GetAllByCompanyIdOrAccountTypeAsync(int CompanyId, string AccountType = "");
        Task<IEnumerable<VoucherDto>> GetAllByCompanyIdOrAccountTypeIdAsync(int TenantId, int CompanyId, int VoucherTypeId);
        Task<IEnumerable<VoucherDto>> GetAllByTenantCompanyIdByVoucherTypeAsync(int TenantId, int CompanyId, string voucherType = null);
        Task<VoucherDto> GetByIdAsync(int id);
        Task<int> AddTransactionAsync(VoucherDto dto);
        Task<int> UpdateTransactionAsync(VoucherDto dto);
    }
}
