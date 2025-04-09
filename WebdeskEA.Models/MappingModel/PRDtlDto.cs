using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.MappingModel
{
    public class PRDtlDto
    {
        public PRDtlDto()
        {}

        //__________ Main Columns __________
        #region Main_Columns

        public int Id { get; set; } // Primary Key
        public int PRId { get; set; } // Foreign Key to PR
        public int ProductId { get; set; } // int, not null
        public int PRDtlQty { get; set; } // int, not null
        public decimal PRDtlPrice { get; set; } // decimal(18, 2), not null
        public decimal PRDtlTotal { get; set; } // decimal(18, 2), not null
        public decimal PRDtlTotalAfterVAT { get; set; } // decimal(18, 2), not null
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string ProductName { get; set; }
        #endregion
    }
}
