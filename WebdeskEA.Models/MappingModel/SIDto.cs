using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Models.MappingModel
{

    public class SIDto : BaseEntity
    {
        public SIDto()
        {
            SODtos = new List<SODto>();
            CustomerDtos = new List<CustomerDto>();
            ProductDtos = new List<ProductDto>();
            SIDtlDtos = new List<SIDtlDto>();
            SIDtlTaxDtos = new List<SIDtlTaxDto>();
            SIVATBreakdownDtos = new List<SIVATBreakdownDto>();
            TaxMasterDtos = new List<TaxMasterDto>();
            CompanyDto = new CompanyDto();
    }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string SICode { get; set; } // nvarchar(50), not null
        public DateTime SIDate { get; set; } = DateTime.Now;
        public int? SOId { get; set; } // int, nullable
        public int CustomerId { get; set; } // int, not null
        public decimal SISubTotal { get; set; } // decimal(18, 2), not null
        public decimal SIDiscount { get; set; } // decimal(18, 2), not null
        public decimal SITotal { get; set; } // decimal(18, 2), not null
        public decimal SITotalAfterVAT { get; set; } // decimal(18, 2), not null
        public int TenantId { get; set; } // int, not null
        public int CompanyId { get; set; } // int, not null
        [ValidateNever]
        public int FinancialYearId { get; set; }
        [ValidateNever]
        public bool isPOS { get; set; } //true pos sa bni invoice , 
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
        public IEnumerable<CustomerDto> CustomerDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SIDtlDto> SIDtlDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SIDtlTaxDto> SIDtlTaxDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<SIVATBreakdownDto> SIVATBreakdownDtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<TaxMasterDto> TaxMasterDtos { get; set; }


        [NotMapped]
        [ValidateNever]
        public CompanyDto CompanyDto { get; set; }

        #endregion
    }

}
