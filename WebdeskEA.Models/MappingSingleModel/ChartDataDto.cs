using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingSingleModel
{
    public class ChartDataDto
    {
        // For daily
        public string Date { get; set; }  // e.g. "2025-02-11"
        public decimal TotalSales { get; set; } // or "TotalPurchase"

        // For monthly, we might also have MonthNumber, MonthName, Year, etc.
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }
        public int Year { get; set; }
        public decimal TotalPurchase { get; set; }
    }
}
