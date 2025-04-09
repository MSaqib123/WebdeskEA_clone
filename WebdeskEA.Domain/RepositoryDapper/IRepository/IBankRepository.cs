using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IBankRepository
    {
        Task<int> AddAsync(BankDto BankDto);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<BankDto>> GetAllAsync();
        Task<IEnumerable<BankDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<BankDto> GetByIdAsync(int id);
        Task<int> UpdateAsync(BankDto BankDto);
    }
}