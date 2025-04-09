using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICountryRepository
    {
        Task<CountryDto> GetByIdAsync(int id);
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<int> AddAsync(CountryDto Dto);
        Task<int> UpdateAsync(CountryDto Dto);
        Task<int> DeletesAsync(int id);
        Task<int> BulkAddAsync(IEnumerable<CountryDto> Dtos);
    }
}
