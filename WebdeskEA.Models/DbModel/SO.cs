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
    public class SO : BaseEntity
    {
        public SO()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        [Key]
        public int Id { get; set; } // Primary key

        [ValidateNever]
        public string SOCode { get; set; } // Sales Order Code

        [Required]
        public DateTime SODate { get; set; } // Sales Order Date

        [Required]
        public int CustomerId { get; set; } // Reference to Customer

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SOSubTotal { get; set; } // Subtotal of the Sales Order

        [Column(TypeName = "decimal(18,2)")]
        public decimal SODiscount { get; set; } = 0.00M; // Discount applied

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SOTotal { get; set; } // Total amount of the Sales Order
        
        public string SONarration { get; set; } // Description or remarks

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SOTotalAfterVAT { get; set; }

        [Required]
        public int TenantId { get; set; } // Reference to Tenant

        [Required]
        public int CompanyId { get; set; } // Reference to Company
        [ValidateNever]
        public int FinancialYearId { get; set; }

        [ValidateNever]
        public bool isholdPOS { get; set; }

        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string CustomerName { get; set; }

        [NotMapped]
        [ValidateNever]
        public bool IsSIExist { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }
}
