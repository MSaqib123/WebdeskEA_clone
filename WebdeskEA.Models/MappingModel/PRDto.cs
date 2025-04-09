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
    public class PRDto : BaseEntity
    {
        public PRDto()
        {
            PODtos = new List<PODto>();
            PIDtos = new List<PIDto>();
            SupplierDtos = new List<SupplierDto>();
            ProductDtos = new List<ProductDto>();
            PRDtlDtos = new List<PRDtlDto>();

            TaxMasterDtos = new List<TaxMasterDto>();
            PRDtlTaxDtos = new List<PRDtlTaxDto>();
            PRVATBreakdownDtos = new List<PRVATBreakdownDto>();
            CompanyDto = new CompanyDto();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; } // Primary Key
        public string PRCode { get; set; } // nvarchar(50), not null
        public DateTime PRDate { get; set; } = DateTime.Now;
        public int? PIId { get; set; } // int, nullable
        public int SupplierId { get; set; } // int, not null
        public decimal PRSubTotal { get; set; } // decimal(18, 2), not null
        public decimal PRDiscount { get; set; } // decimal(18, 2), not null
        public decimal PRTotal { get; set; } // decimal(18, 2), not null
        [ValidateNever]
        public decimal PRTotalAfterVAT { get; set; }
        public int TenantId { get; set; } // int, not null
        public int CompanyId { get; set; } // int, not null
        public int FinancialYearId { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string SupplierName { get; set; }
        #endregion

        #region Note_Mapped
        [NotMapped]
        [ValidateNever]
        public IEnumerable<PODto> PODtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<PIDto> PIDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SupplierDto> SupplierDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<PRDtlDto> PRDtlDtos { get; set; }


        [NotMapped]
        [ValidateNever]
        public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }


        [NotMapped]
        [ValidateNever]
        public IEnumerable<PRDtlTaxDto> PRDtlTaxDtos { get; set; }
        
        [NotMapped]
        [ValidateNever]
        public IEnumerable<PRVATBreakdownDto> PRVATBreakdownDtos { get; set; }
        
        [NotMapped]
        [ValidateNever]
        public CompanyDto CompanyDto { get; set; }

        #endregion
    }
}
