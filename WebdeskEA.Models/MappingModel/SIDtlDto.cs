using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{

    public class SIDtlDto
    {
        public SIDtlDto()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public int SIId { get; set; } // int, not null
        public int ProductId { get; set; } // int, not null
        public int SIDtlQty { get; set; } // int, not null
        public decimal SIDtlPrice { get; set; } // decimal(18, 2), not null
        public decimal SIDtlTotal { get; set; } // decimal(18, 2), not null
        public decimal SIDtlTotalAfterVAT { get; set; } // decimal(18, 2), not null
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string ProductName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

}
