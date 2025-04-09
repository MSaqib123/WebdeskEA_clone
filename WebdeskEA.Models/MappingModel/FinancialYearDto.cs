using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.MappingModel
{
    public class FinancialYearDto:BaseEntity
    {
        public FinancialYearDto()
        {
            TenantDtoList = new List<TenantDto>();
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [ValidateNever]
        public string FYDescription { get; set; }
        public string CodePrefix { get; set; }
        [ValidateNever]
        public string FYCode { get; set; }
        public DateTime FYStartDate { get; set; } = DateTime.Now;
        public DateTime FYEndDate { get; set; } = DateTime.Now;
        public bool isCurrentYear { get; set; }
        public bool isLock { get; set; }
        [ValidateNever]
        public int TenantId { get; set; }
        [ValidateNever]
        public int CompanyId { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string TenantName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        [ValidateNever]
        public IEnumerable<TenantDto> TenantDtoList { get; set; }
        #endregion

    }
}
