using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.Models.DbModel
{
    public class FinancialYear : BaseEntity
    {
        
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string FYDescription { get; set; }
        public string CodePrefix { get; set; }
        public string FYCode { get; set; }
        public DateTime FYStartDate { get; set; }
        public DateTime FYEndDate { get; set; }
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
        public string TenantName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion



    }
}
