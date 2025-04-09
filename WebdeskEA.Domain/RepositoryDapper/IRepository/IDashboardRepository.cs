using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.MappingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using WebdeskEA.Models.MappingSingleModel;

namespace WebdeskEA.Domain.RepositoryDapper.IRepository
{
    public interface IDashboardRepository //: IRepository<PO>
    {
        Task<DashboardDto> GetAllDashboardStatisticsAsync(int tenantId, int companyId, int financialYearId);

    }

}
