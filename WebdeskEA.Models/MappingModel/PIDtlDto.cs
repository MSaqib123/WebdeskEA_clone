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
    public class PIDtlDto
    {
        public PIDtlDto()
        {}

        //__________ Main Columns __________
        #region Main_Columns

        public int Id { get; set; } // Primary Key
        public int PIId { get; set; } // Foreign Key to PI
        public int ProductId { get; set; } // int, not null
        public int PIDtlQty { get; set; } // int, not null
        public decimal PIDtlPrice { get; set; } // decimal(18, 2), not null
        public decimal PIDtlTotal { get; set; } // decimal(18, 2), not null
        public decimal PIDtlTotalAfterVAT { get; set; } // decimal(18, 2), not null
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string ProductName { get; set; }
        #endregion
    }
}
