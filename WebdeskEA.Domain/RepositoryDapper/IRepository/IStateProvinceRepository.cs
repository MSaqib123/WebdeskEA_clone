using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IStateProvinceRepository
    {
        Task<StateProvinceDto> GetByIdAsync(int id);
        Task<IEnumerable<StateProvinceDto>> GetAllListAsync();
        Task<IEnumerable<StateProvinceDto>> GetStateProvinceByCountryIdAsync(int countryId);
        Task<int> AddAsync(StateProvinceDto Dto);
        Task<int> UpdateAsync(StateProvinceDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<StateProvinceDto> Dtos);
    }
}
