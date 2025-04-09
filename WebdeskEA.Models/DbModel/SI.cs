using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{

    public class SI : BaseEntity
    {
        public SI()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string SICode { get; set; } // nvarchar(50), not null
        public DateTime SIDate { get; set; } // datetime, not null
        public int? SOId { get; set; } // int, nullable
        public int CustomerId { get; set; } // int, not null
        public decimal SISubTotal { get; set; } // decimal(18, 2), not null
        public decimal SIDiscount { get; set; } // decimal(18, 2), not null
        public decimal SITotal { get; set; } // decimal(18, 2), not null
        public decimal SITotalAfterVAT { get; set; } // decimal(18, 2), not null
        public int TenantId { get; set; } // int, not null
        public int CompanyId { get; set; } // int, not null
        [ValidateNever]
        public int FinancialYearId { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string CustomerName { get; set; }
        [NotMapped]
        [ValidateNever]
        public bool IsReturnExist { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

}
