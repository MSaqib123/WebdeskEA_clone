using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;
using System.Data;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ITenantRepository
    {
        Task<TenantDto> GetByIdAsync(int id);
        Task<IEnumerable<TenantDto>> GetAllAsync();
        Task<int> AddAsync(TenantDto Dto, IDbTransaction transaction = null);
        Task<int> UpdateAsync(TenantDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<TenantDto> Dtos);
    }
}
