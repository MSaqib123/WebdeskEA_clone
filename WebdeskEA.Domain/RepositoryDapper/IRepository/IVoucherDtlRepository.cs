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
    public interface IVoucherDtlRepository
    {
        Task<IEnumerable<VoucherDtlDto>> GetAllAsync();
        Task<int> AddAsync(VoucherDtlDto Dto);
        Task<int> UpdateAsync(VoucherDtlDto Dto);
        Task<int> DeletesAsync(int id);
        Task<IEnumerable<VoucherDtlDto>> GetByNameAsync(string name);
        Task<int> BulkAddProcAsync(IEnumerable<VoucherDtlDto> Dtos);
        Task<IEnumerable<VoucherDtlDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<int> DeleteByVoucherIdAsync(int id);
        Task<VoucherDtlDto> GetByIdAsync(int id);
        Task<IEnumerable<VoucherDtlDto>> GetAllByVoucherIdCompanyAndTenantIdAsync(int Id, int TenantId, int CompanyId);
    }
}
