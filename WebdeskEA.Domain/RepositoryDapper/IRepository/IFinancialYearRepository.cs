using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IFinancialYearRepository
    {
        #region Get
        Task<FinancialYearDto> GetByIdAsync(int id);
        Task<IEnumerable<FinancialYearDto>> GetAllAsync();
        Task<IEnumerable<FinancialYearDto>> GetAllByCompanyId(int CompanyId);
        Task<IEnumerable<FinancialYearDto>> GetAllByTenantCompanyIdAsync(int TenantId, int CompanyId);
        #endregion

        #region Add
        Task<int> AddAsync(FinancialYearDto Dto);
        #endregion

        #region Update
        Task<int> UpdateAsync(FinancialYearDto Dto);
        #endregion

        #region Delete
        Task<int> DeleteAsync(int id);
        Task<int> UpdateIsLocakByCompanyAndFYIdAsync(int id, int CompanyId);
        #endregion

    }
}
