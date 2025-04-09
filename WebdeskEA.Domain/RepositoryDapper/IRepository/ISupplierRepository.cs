using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ISupplierRepository
    {
        Task<int> AddAsync(SupplierDto SupplierDto);
        Task<int> BulkAddSupplierAsync(IEnumerable<SupplierDto> Dtos, PackageDto packDto);
        Task<int> DeleteAsync(int id);
        Task<int> DeleteByPackageIdAsync(int PackageId);
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<IEnumerable<SupplierDto>> GetAllByPackageIdAsync(int id);
        Task<IEnumerable<SupplierDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        Task<SupplierDto> GetByIdAsync(int id);
        Task<int> UpdateAsync(SupplierDto SupplierDto);
    }
}