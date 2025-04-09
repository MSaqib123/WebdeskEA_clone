using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IPackageRepository
    {
        Task<PackageDto> GetByIdAsync(int id);
        Task<IEnumerable<PackageDto>> GetAllAsync();
        Task<int> AddAsync(PackageDto packageDto);
        Task<int> UpdateAsync(PackageDto packageDto);
        Task<int> DeleteAsync(int id);
    }
}
