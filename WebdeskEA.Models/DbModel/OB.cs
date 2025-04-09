using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.DbModel
{
    public class OB : BaseEntity
    {

        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        public string OBCode { get; set; } = string.Empty;
        public DateTime OBDate { get; set; }
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public int FinancialYearId { get; set; }
        #endregion

        //__________ Joined Columns __________
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string SupplierName { get; set; }
        #endregion
    }
}
