using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IModuleRepository
    {
        #region Get
        Task<ModuleDto> GetByIdAsync(int id);
        Task<IEnumerable<ModuleDto>> GetAllAsync();
        #endregion

        #region Add
        Task<int> AddAsync(ModuleDto Dto);
        Task<int> BulkAddAsync(IEnumerable<ModuleDto> Dtos);
        #endregion

        #region Update
        Task<int> UpdateAsync(ModuleDto Dto);
        #endregion

        #region Delete
        Task<int> DeletesAsync(int id);
        #endregion

        #region Not_Used
        Task<IEnumerable<ModuleDto>> GetByNameAsync(string name);
        #endregion
    }
}
