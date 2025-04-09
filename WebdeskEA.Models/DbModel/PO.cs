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
    public class PO : BaseEntity
    {
        
        //__________ Main Columns __________
        #region Main_Columns

        [Key]
        public int Id { get; set; } // Primary Key with Auto Increment

        [Required]
        [MaxLength(50)]
        public string POCode { get; set; } // Purchase Order Code

        [Required]
        public DateTime PODate { get; set; } // Purchase Order Date

        [Required]
        public int SupplierId { get; set; } // Supplier Id (Assumed to be linked to a Suppliers table)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal POSubTotal { get; set; } // Purchase Order Subtotal

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PODiscount { get; set; } // Discount on the Purchase Order

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal POTotal { get; set; } // Total Amount of the Purchase Order

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal POTotalAfterVAT { get; set; } // Total Amount of the Purchase Order

        [MaxLength(255)]
        [ValidateNever]
        public string PONarration { get; set; } // Narration/Comments for the Purchase Order

        [Required]
        public int TenantId { get; set; } // Tenant Id (multi-tenancy)

        [Required]
        public int CompanyId { get; set; } // Company Id
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
        public bool IsPIExist { get; set; }
        #endregion
    }
}
