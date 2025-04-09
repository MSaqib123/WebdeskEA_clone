using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICompanyUserRepository
    {
        Task<CompanyUserDto> GetCompanyUserByCompanyIdAsync(int id);
        Task<IEnumerable<CompanyUserDto>> GetAllCompanyUserByCompanyIdAsync(int companyId);
        Task<CompanyUserDto> GetCompanyUserWithUpdateBaseAsync(int companyId);
        Task<IEnumerable<CompanyUserDto>> GetAllCompanyUsersAsync();
        Task<IEnumerable<ApplicationUserDto>> GetAllUsersNEICUAsync();
        Task<int> AddCompanyUserAsync(CompanyUserDto CompanyUserDto);
        Task<int> BulkAddCompanyUserAsync(IEnumerable<CompanyUserDto> CompanyUserDtos);
        Task<int> UpdateCompanyUserAsync(CompanyUserDto CompanyUserDto);
        Task<int> BulkUpdateCompanyUserAsync(int CompanyId,IEnumerable<CompanyUserDto> CompanyUserDtos);
        Task<int> DeleteCompanyUserAsync(int id);

        //______ Not in Used _____
        Task<IEnumerable<CompanyUserDto>> GetCompanyUserByNameAsync(string name);
        Task<IEnumerable<CompanyUserDto>> BulkLoadCompanyUserAsync(string procedure, object parameters = null);
        Task<IEnumerable<CompanyUserDto>> GetPaginatedCompanyUserAsync(int pageIndex, int pageSize, string filter);
    }
}
