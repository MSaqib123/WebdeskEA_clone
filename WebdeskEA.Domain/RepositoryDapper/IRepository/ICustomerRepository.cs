using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<IEnumerable<CustomerDto>> GetAllByPackageIdAsync(int id);
        Task<CustomerDto> GetByIdAsync(int id);

        Task<int> AddAsync(CustomerDto CustomerDto);
        Task<int> BulkAddCustomerAsync(IEnumerable<CustomerDto> Dtos, PackageDto packDto);
        Task<int> UpdateAsync(CustomerDto CustomerDto);

        Task<int> DeleteAsync(int id);
        Task<int> DeleteByPackageIdAsync(int PackageId);
        Task<IEnumerable<CustomerDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
    }
}