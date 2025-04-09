using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.MappingModel
{
    public class SODto : BaseEntity
    {
        public SODto()
        {
            CustomerDtos = new List<CustomerDto>();
            ProductDtos = new List<ProductDto>();
            SODtlDtos = new List<SODtlDto>();
            TaxMasterDtos = new List<TaxMasterDto>();
            SODtlTaxDtos = new List<SODtlTaxDto>();
            SOVATBreakdownDtos = new List<SOVATBreakdownDto>();
            CompanyDto = new CompanyDto();
        }

        //__________ Main Columns __________
        #region Main_Columns
        [Key]
        public int Id { get; set; } 

        [ValidateNever]
        public string SOCode { get; set; }

        [Required]
        public DateTime SODate { get; set; }  = DateTime.Now;

        [Required]
        public int CustomerId { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SOSubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SODiscount { get; set; } = 0.00M; 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SOTotal { get; set; }

        [ValidateNever]
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
        [NotMapped]
        [ValidateNever]
        public IEnumerable<CustomerDto> CustomerDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SODtlTaxDto> SODtlTaxDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SOVATBreakdownDto> SOVATBreakdownDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SODtlDto> SODtlDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public CompanyDto CompanyDto { get; set; }
        #endregion
    }
}
