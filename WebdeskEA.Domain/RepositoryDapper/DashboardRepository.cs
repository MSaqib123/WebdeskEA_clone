using WebdeskEA.DataAccess.DapperFactory;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.DbModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebdeskEA.Models.MappingModel;
using WebdeskEA.Domain.CommonMethod;
using WebdeskEA.Domain.RepositoryEntity.IRepository;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;
using WebdeskEA.Models.MappingSingleModel;

namespace WebdeskEA.Domain.RepositoryDapper
{
    public class DashboardRepository : Repository<DashboardDto>, IDashboardRepository
    {
        private readonly IMapper _mapper;
        private readonly IProductCOARepository _productCOARepository;
        private readonly IOSDtlRepository _OSDtlRepository;
        private readonly IPIRepository _PIRepository;

        public DashboardRepository(IDapperDbConnectionFactory dbConnectionFactory,
            IMapper mapper, 
            IProductCOARepository productCOARepository,
            IOSDtlRepository OSDtlRepository,
            IPIRepository PIRepository)
            : base(dbConnectionFactory)
        {
            _mapper = mapper;
            _productCOARepository = productCOARepository;
            _OSDtlRepository = OSDtlRepository;
            _PIRepository = PIRepository;
        }

        public async Task<DashboardDto> GetAllDashboardStatisticsAsync(int tenantId,int companyId,int financialYearId)
        {
            // 1) Prepare the stored procedure & parameters
            var procedure = "spDashboard_GetAllStatistics";
            var parameters = new
            {
                TenantId = tenantId,
                CompanyId = companyId,
                FinancialYearId = financialYearId
            };

            // 2) Call the procedure and handle multiple result sets
            using (var multi = await _dbConnection.QueryMultipleAsync(
                                    procedure,
                                    parameters,
                                    commandType: CommandType.StoredProcedure))
            {
                // Initialize the main DTO
                var dashboardData = new DashboardDto();

                // 3) First result => single row summary
                dashboardData.Summary = multi.ReadFirstOrDefault<DashboardSummaryDto>();

                // 4) Second result => daily sales
                dashboardData.DailySales = multi.Read<ChartDataDto>().ToList();

                // 5) Third => monthly sales
                dashboardData.MonthlySales = multi.Read<ChartDataDto>().ToList();

                // 6) Fourth => daily purchase
                dashboardData.DailyPurchase = multi.Read<ChartDataDto>().ToList();

                // 7) Fifth => monthly purchase
                dashboardData.MonthlyPurchase = multi.Read<ChartDataDto>().ToList();

                // 8) Sixth => bubble chart (if any)
                dashboardData.BubbleChart = multi.Read<BubbleChartDto>().ToList();


                //=========== Profite ==============
                dashboardData.MonthlyProfitLoss = await GetMonthlyProfitLossAsync(
                     tenantId, companyId, financialYearId);

                dashboardData.WeeklyProfitLoss = await GetWeeklyProfitLossAsync(
                     tenantId, companyId, financialYearId);

                dashboardData.YearlyProfitLoss = await GetYearlyProfitLossAsync(
                     tenantId, companyId, financialYearId);


                return dashboardData;
            }
        }

        public async Task<List<ProfitLossDataDto>> GetMonthlyProfitLossAsync(int tenantId,int companyId,int financialYearId)
        {
            var procedure = "spDashboard_GetMonthlyProfitLoss";  // Your stored proc
            var parameters = new
            {
                TenantId = tenantId,
                CompanyId = companyId,
                FinancialYearId = financialYearId
            };

            var result = await _dbConnection.QueryAsync<ProfitLossDataDto>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<ProfitLossDataDto>> GetWeeklyProfitLossAsync(int tenantId, int companyId, int financialYearId)
        {
            var procedure = "spDashboard_GetWeeklyProfitLoss";  // Your stored proc
            var parameters = new
            {
                TenantId = tenantId,
                CompanyId = companyId,
                FinancialYearId = financialYearId
            };

            var result = await _dbConnection.QueryAsync<ProfitLossDataDto>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<ProfitLossDataDto>> GetYearlyProfitLossAsync(int tenantId, int companyId, int financialYearId)
        {
            var procedure = "spDashboard_GetYearlyProfitLoss";  // Your stored proc
            var parameters = new
            {
                TenantId = tenantId,
                CompanyId = companyId,
                FinancialYearId = financialYearId
            };

            var result = await _dbConnection.QueryAsync<ProfitLossDataDto>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }



    }
}
