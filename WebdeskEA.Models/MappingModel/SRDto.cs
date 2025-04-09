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

    public class SRDto : BaseEntity
    {
        public SRDto()
        {
            SODtos = new List<SODto>();
            SIDtos = new List<SIDto>();
            CustomerDtos = new List<CustomerDto>();
            ProductDtos = new List<ProductDto>();
            SRDtlDtos = new List<SRDtlDto>();
            SRDtlTaxDtos = new List<SRDtlTaxDto>();
            SRVATBreakdownDtos = new List<SRVATBreakdownDto>();
            TaxMasterDtos = new List<TaxMasterDto>();
            CompanyDto = new CompanyDto();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string SRCode { get; set; } // nvarchar(50), not null
        public DateTime SRDate { get; set; } = DateTime.Now;
        public int? SIId { get; set; } // int, nullable
        public int CustomerId { get; set; } // int, not null
        public decimal SRSubTotal { get; set; } // decimal(18, 2), not null
        public decimal SRDiscount { get; set; } // decimal(18, 2), not null
        public decimal SRTotal { get; set; } // decimal(18, 2), not null
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SRTotalAfterVAT { get; set; }

        public int TenantId { get; set; } // int, not null
        public int CompanyId { get; set; } // int, not null
        [ValidateNever]
        public int FinancialYearId { get; set; }
        #endregion

        //__________ Joined Columns -----
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
        [NotMapped]
        [ValidateNever]
        public IEnumerable<SODto> SODtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SIDto> SIDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<CustomerDto> CustomerDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SRDtlDto> SRDtlDtos { get; set; }

        
        [NotMapped]
        [ValidateNever]
        public IEnumerable<SRDtlTaxDto> SRDtlTaxDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SRVATBreakdownDto> SRVATBreakdownDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }
        
        [NotMapped]
        [ValidateNever]
        public CompanyDto CompanyDto { get; set; }

        #endregion
    }

}
