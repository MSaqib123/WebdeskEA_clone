using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{

    public class TaxMaster : BaseEntity
    {
        public TaxMaster()
        {
        }

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string TaxName { get; set; }
        public int COAId { get; set; }
        public decimal TaxValue { get; set; }
        public bool IsInclusive { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsFix { get; set; }
        public int TenantId { get; set; }
        public int TenantCompanyId { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string AccountName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

    
}
