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
    public class PI : BaseEntity
    {

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; } // Primary Key
        public string PICode { get; set; } // nvarchar(50), not null
        public DateTime PIDate { get; set; } // datetime, not null
        public int? POId { get; set; } // int, nullable
        public int SupplierId { get; set; } // int, not null
        public decimal PISubTotal { get; set; } // decimal(18, 2), not null
        public decimal PIDiscount { get; set; } // decimal(18, 2), not null
        public decimal PITotal { get; set; } // decimal(18, 2), not null
        public decimal PITotalAfterVAT { get; set; } // decimal(18, 2), not null
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
        [NotMapped]
        [ValidateNever]
        public bool IsReturnExist { get; set; }
        #endregion
    }
}
