using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICityRepository
    {
        Task<CityDto> GetByIdAsync(int id);
        Task<IEnumerable<CityDto>> GetAllListAsync();
        Task<IEnumerable<CityDto>> GetCityByStateIdAsync(int StateId);
        Task<int> AddAsync(CityDto Dto);
        Task<int> UpdateAsync(CityDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<CityDto> Dtos);
    }
}
