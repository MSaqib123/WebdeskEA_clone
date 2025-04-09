using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IPackagePermissionRepository
    {
        #region Get
        Task<PackagePermissionDto> GetByIdAsync(int id);
        Task<IEnumerable<PackagePermissionDto>> GetAllByPackageIdAsync(int id);
        Task<IEnumerable<PackagePermissionDto>> GetAllAsync();
        Task<PackagePermissionDto> GetPackagePermissionForUpdateBaseAsync(int PackageId =0);

        #endregion

        #region Add
        Task<int> AddAsync(PackagePermissionDto packagePermissionDto);
        Task<int> BulkAddPackagePermissionAsync(IEnumerable<PackagePermissionDto> Dtos, PackageDto packDto);
        #endregion

        #region Update
        Task<int> UpdateAsync(PackagePermissionDto packagePermissionDto);
        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> DeleteByPackageIdAsync(int PackageId);

        #endregion

    }

}
