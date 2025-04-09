using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IGlobalSettingRepository
    {
        Task<IEnumerable<GlobalSettingsDto>> GetAllAsync();
        Task<GlobalSettingsDto> GetByIdAsync(int id);
        Task<GlobalSettingsDto> GetByTenantCompanyUserKeyAsync(int? tenantId, int? companyId, string userId, string key);
        Task<int> AddAsync(GlobalSettingsDto dto);
        Task<int> UpdateAsync(GlobalSettingsDto dto);
        Task<int> DeleteAsync(int id);
    }
}