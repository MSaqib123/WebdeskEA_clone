using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class COADto : BaseEntity
    {
        public COADto()
        {
            //---- Main Record which will be loaded base on the Coatypeid ----
            COADtoList = new List<COADto>();
            CoatypeDtoList = new List<COATypeDto>();
            CoaCategoryDtoList = new List<CoaCategoryDto>();
            TenantDtoList = new List<TenantDto>();
            CompanyDtoList = new List<CompanyDto>();
            CoaTransactionTypeList = new SelectList(new List<KeyValuePair<string, string>>()); // Empty list to avoid null
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string? AccountCode { get; set; }
        public string? Code { get; set; }
        [Required]
        public string? AccountName { get; set; }
        public int? ParentAccountId { get; set; }
        public int CoaCategoryId { get; set; }
        [Required]
        public int? CoatypeId { get; set; }
        [Required]
        public string? CoaTranType { get; set; }
        public string? Description { get; set; }
        public bool Transable { get; set; }
        public int? LevelNo { get; set; }
        public int TenantId { get; set; } 
        public int TenantCompanyId { get; set; } 
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns

        [ValidateNever]
        public string? ParentAccountName { get; set; }

        [ValidateNever]
        public string? COATypeName { get; set; }

        [ValidateNever]
        public string? TenantName { get; set; }

        [NotMapped]
        public string? CompanyName { get; set; }

        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        [ValidateNever]
        public IEnumerable<CoaCategoryDto> CoaCategoryDtoList { get; set; }

        [ValidateNever]
        public IEnumerable<COATypeDto> CoatypeDtoList { get; set; }


        [ValidateNever]
        public IEnumerable<COADto> COADtoList { get; set; }

        [ValidateNever]
        public IEnumerable<TenantDto> TenantDtoList { get; set; }

        [ValidateNever]
        public IEnumerable<CompanyDto> CompanyDtoList { get; set; }

        [ValidateNever]
        public SelectList CoaTransactionTypeList { get; set; }

        [ValidateNever]
        public List<COADto> Children { get; set; } = new List<COADto>();
        #endregion
    }

}
