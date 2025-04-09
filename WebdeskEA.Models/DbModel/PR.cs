using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class PR : BaseEntity
    {

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; } // Primary Key
        public string PRCode { get; set; } // nvarchar(50), not null
        public DateTime PRDate { get; set; } = DateTime.Now;// datetime, not null
        public int? PIId { get; set; } // int, nullable
        public int SupplierId { get; set; } // int, not null
        public decimal PRSubTotal { get; set; } // decimal(18, 2), not null
        public decimal PRDiscount { get; set; } // decimal(18, 2), not null
        public decimal PRTotal { get; set; } // decimal(18, 2), not null
        public decimal PRTotalAfterVAT { get; set; }
        public int TenantId { get; set; } // int, not null
        public int CompanyId { get; set; } // int, not null
        [ValidateNever]
        public int FinancialYearId { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string SupplierName { get; set; }
        #endregion
    }
}
