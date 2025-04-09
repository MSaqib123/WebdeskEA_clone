using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingSingleModel
{
    public class DashboardDto
    {
        public DashboardSummaryDto Summary { get; set; }

        public List<ChartDataDto> DailySales { get; set; }
        public List<ChartDataDto> MonthlySales { get; set; }
        public List<ChartDataDto> WeeklySales { get; set; }
        public List<ChartDataDto> YearlySales { get; set; }

        public List<ChartDataDto> DailyPurchase { get; set; }
        public List<ChartDataDto> MonthlyPurchase { get; set; }
        public List<ChartDataDto> WeeklyPurchase { get; set; }
        public List<ChartDataDto> YearlyPurchase { get; set; }


        public List<BubbleChartDto> BubbleChart { get; set; }

        // NEW: Monthly Profit/Loss
        public List<ProfitLossDataDto> MonthlyProfitLoss { get; set; }
        public List<ProfitLossDataDto> WeeklyProfitLoss { get; set; }
        public List<ProfitLossDataDto> YearlyProfitLoss { get; set; }

        public DashboardDto()
        {
            DailySales = new List<ChartDataDto>();
            MonthlySales = new List<ChartDataDto>();
            WeeklySales = new List<ChartDataDto>();
            YearlySales = new List<ChartDataDto>();

            DailyPurchase = new List<ChartDataDto>();
            MonthlyPurchase = new List<ChartDataDto>();
            WeeklyPurchase = new List<ChartDataDto>();
            YearlyPurchase = new List<ChartDataDto>();

            BubbleChart = new List<BubbleChartDto>();

            // Initialize new list
            MonthlyProfitLoss = new List<ProfitLossDataDto>();
        }
    }

    public class ProfitLossDataDto
    {
        public int Year { get; set; }
        public int MonthNumber { get; set; }
        public string MonthName { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal ProfitOrLoss { get; set; }
    }

}
