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
    public class PODtl
    {
        public PODtl()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns

        [Key]
        public int Id { get; set; } // Primary Key with Auto Increment

        [Required]
        public int POId { get; set; } // Reference to the Purchase Order (POs table)

        [Required]
        public int ProductId { get; set; } // Reference to the Product

        [Required]
        public int PODtlQty { get; set; } // Quantity for this Purchase Order Detail

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PODtlPrice { get; set; } // Price for each unit in the Purchase Order Detail

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PODtlTotal { get; set; } // Total amount for this detail line (PODtlQty * PODtlPrice)


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PODtlTotalAfterVAT { get; set; } // Total amount for this detail line (PODtlQty * PODtlPrice)

        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        #endregion
    }
}
