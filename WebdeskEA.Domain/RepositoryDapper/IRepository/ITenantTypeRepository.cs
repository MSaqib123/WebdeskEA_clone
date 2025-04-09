using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ITenantTypeRepository
    {
        Task<TenantTypeDto> GetByIdAsync(int id);
        Task<IEnumerable<TenantTypeDto>> GetAllAsync();
        Task<int> AddAsync(TenantTypeDto Dto);
        Task<int> UpdateAsync(TenantTypeDto Dto);
        Task<int> DeleteAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<TenantTypeDto> Dtos);
    }
}
