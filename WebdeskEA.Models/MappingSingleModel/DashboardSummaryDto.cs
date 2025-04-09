using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingSingleModel
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalProducts { get; set; }

        public int TotalSalesInvoices { get; set; }
        public decimal SumSITotal { get; set; }
        public decimal SumSITotalAfterVAT { get; set; }

        public int TotalPurchaseInvoices { get; set; }
        public decimal SumPITotal { get; set; }
        public decimal SumPITotalAfterVAT { get; set; }
    }
}
