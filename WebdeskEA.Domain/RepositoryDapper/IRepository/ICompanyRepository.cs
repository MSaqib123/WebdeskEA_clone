using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICompanyRepository
    {
        Task<CompanyDto> GetByIdAsync(int id);
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<int> AddAsync(CompanyDto companyDto);
        Task<int> UpdateAsync(CompanyDto companyDto);

        Task<int> DeleteAsync(int id);
        Task<IEnumerable<CompanyDto>> GetByNameAsync(string name);
        Task<int> BulkInsertAsync(IEnumerable<CompanyDto> companyDtos);
        Task<IEnumerable<CompanyDto>> BulkLoadAsync(string procedure, object parameters = null);
        Task<IEnumerable<CompanyDto>> GetPaginatedAsync(int pageIndex, int pageSize, string filter);
        Task<IEnumerable<CompanyDto>> GetAllByTenantIdAsync(int TenantId);
        Task<IEnumerable<CompanyDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
    }
}
