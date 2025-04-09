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

    public class SR : BaseEntity
    {
        public SR()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string SRCode { get; set; } // nvarchar(50), not null
        public DateTime SRDate { get; set; } // datetime, not null
        public int? SIId { get; set; } // int, nullable
        public int CustomerId { get; set; } // int, not null
        public decimal SRSubTotal { get; set; } // decimal(18, 2), not null
        public decimal SRDiscount { get; set; } // decimal(18, 2), not null
        public decimal SRTotal { get; set; } // decimal(18, 2), not null
        public decimal SRTotalAfterVAT { get; set; }
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
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

}
