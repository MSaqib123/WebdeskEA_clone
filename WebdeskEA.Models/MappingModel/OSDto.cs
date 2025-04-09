using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.DbModel
{
    public class OSDto : BaseEntity
    {
        public OSDto()
        {
            ProductDtos = new List<ProductDto>();
            OSDtlDtos = new List<OSDtlDto>();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string OSCode { get; set; } = string.Empty;
        public DateTime OSDate { get; set; } = DateTime.Now;
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public int FinancialYearId { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string FinancialYearName { get; set; }
        #endregion


        //__________ List and Single Object__________
        #region Note_Mapped
        [NotMapped]
        [ValidateNever]
        public IEnumerable<ProductDto> ProductDtos { get; set; }


        [NotMapped]
        [ValidateNever]
        public IEnumerable<OSDtlDto> OSDtlDtos { get; set; }
        #endregion
    }
}
