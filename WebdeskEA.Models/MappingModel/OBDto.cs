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
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.DbModel
{
    public class OBDto : BaseEntity
    {
        public OBDto()
        {
            CoaTransactionTypeList = new SelectList(new List<KeyValuePair<string, string>>()); // Empty list to avoid null
            COADtos = new List<COADto>();
            OBDtlDtos = new List<OBDtlDto>();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string OBCode { get; set; } = string.Empty;
        public DateTime OBDate { get; set; } = DateTime.Now;
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
        public SelectList CoaTransactionTypeList { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<COADto> COADtos { get; set; }

        [NotMapped]
        [ValidateNever]
        public IEnumerable<OBDtlDto> OBDtlDtos { get; set; }
        #endregion

    }
}
