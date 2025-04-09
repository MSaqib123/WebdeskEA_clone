using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICOATypeRepository
    {
        Task<COATypeDto> GetByIdAsync(int id);
        Task<IEnumerable<COATypeDto>> GetAllListAsync();
        Task<int> AddAsync(COATypeDto Dto);
        Task<int> UpdateAsync(COATypeDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<COATypeDto> Dtos);
    }
}
