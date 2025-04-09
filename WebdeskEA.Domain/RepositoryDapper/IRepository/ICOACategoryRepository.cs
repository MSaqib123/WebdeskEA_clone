using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICOACategoryRepository
    {
        Task<CoaCategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<CoaCategoryDto>> GetAllListAsync();
        Task<int> AddAsync(CoaCategoryDto Dto);
        Task<int> UpdateAsync(CoaCategoryDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<CoaCategoryDto> Dtos);
    }
}
