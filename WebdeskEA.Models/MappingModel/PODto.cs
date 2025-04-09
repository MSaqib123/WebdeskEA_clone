using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Models.MappingModel
{
    public class PODto : BaseEntity
    {
        public PODto()
        {
            SupplierDtos = new List<SupplierDto>();
            ProductDtos = new List<ProductDto>();
            PODtlDtos = new List<PODtlDto>();
            PODtlTaxDtos = new List<PODtlTaxDto>();
            POVATBreakdownDtos = new List<POVATBreakdownDto>();
            TaxMasterDtos = new List<TaxMasterDto>();
            CompanyDto = new CompanyDto();
        }

        //__________ Main Columns __________
        #region Main_Columns

        [Key]
        public int Id { get; set; } // Primary Key with Auto Increment

        [Required]
        [MaxLength(50)]
        public string POCode { get; set; } // Purchase Order Code


        [Required]
        public DateTime PODate { get; set; } = DateTime.Now;

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

        [ValidateNever]
        [Column(TypeName = "decimal(18,2)")]
        public decimal POTotalAfterVAT { get; set; } // Total Amount of the Purchase Order

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

        [NotMapped]
        [ValidateNever]
        public string ProductName { get; set; } // Purchase Order Code
        #endregion



        //__________ List and Single Object__________
        #region Note_Mapped
        [NotMapped]
        [ValidateNever]
        public IEnumerable<SupplierDto> SupplierDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }


        [NotMapped]
        [ValidateNever]
        public IEnumerable<PODtlDto> PODtlDtos { get; set; }
        
        
        [NotMapped]
        [ValidateNever]
        public IEnumerable<PODtlTaxDto> PODtlTaxDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<POVATBreakdownDto> POVATBreakdownDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public CompanyDto CompanyDto { get; set; }
        #endregion

    }
}
