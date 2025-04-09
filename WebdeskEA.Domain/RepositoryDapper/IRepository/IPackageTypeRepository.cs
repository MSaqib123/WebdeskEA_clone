using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IPackageTypeRepository
    {
        Task<PackageTypeDto> GetByIdAsync(int id);
        Task<IEnumerable<PackageTypeDto>> GetAllAsync();
        Task<int> AddAsync(PackageTypeDto packageTypeDto);
        Task<int> UpdateAsync(PackageTypeDto packageTypeDto);
        Task<int> DeleteAsync(int id);
    }
}
