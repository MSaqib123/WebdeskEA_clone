using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface ICompanyBusinessCategoryRepository
    {
        Task<IEnumerable<CompanyBusinessCategoryDto>> GetAllCompanyBusinessCategoryAsync();
        Task<CompanyBusinessCategoryDto> GetCompanyBusinessCategoryByIdAsync(int id);
        Task<IEnumerable<CompanyBusinessCategoryDto>> GetAllCompanyBusinessCategoryByCompanyIdAsync(int companyId);

        Task<int> AddCompanyBusinessCategoryAsync(CompanyBusinessCategoryDto CompanyUserDto);
        Task<int> BulkAddCompanyBusinessCategoryAsync(IEnumerable<CompanyBusinessCategoryDto> CompanyUserDtos);

        Task<int> UpdateCompanyBusinessCategoryAsync(CompanyBusinessCategoryDto CompanyUserDto);
        Task<int> BulkUpdateCompanyBusinessCategoryAsync(int CompanyId,IEnumerable<CompanyBusinessCategoryDto> CompanyUserDtos);

        Task<int> DeleteCompanyBusinessCategoryAsync(int id);


        //______ Not in Used _____
        Task<CompanyBusinessCategoryDto> GetCompanyBusinessCategoryWithUpdateBaseAsync(int companyId);
        //Task<IEnumerable<ApplicationUserDto>> GetAllUsersNEICUAsync();
        //Task<IEnumerable<CompanyUserDto>> GetCompanyUserByNameAsync(string name);
        //Task<IEnumerable<CompanyUserDto>> BulkLoadCompanyUserAsync(string procedure, object parameters = null);
        //Task<IEnumerable<CompanyUserDto>> GetPaginatedCompanyUserAsync(int pageIndex, int pageSize, string filter);
    }
}
