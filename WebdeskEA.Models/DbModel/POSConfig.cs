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

    public class POSConfig : BaseEntity
    {
        //__________ Main Columns __________
        #region Main_Columns
        public int Id { get; set; }
        [Required]
        public int DefaultCustomer { get; set; }
        public decimal DefaultTax { get; set; }
        public bool? IsCurrentActive { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int CompanyId { get; set; }

        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        [ValidateNever]
        public string CustomerName { get; set; }
        #endregion


        //__________ List and Single Object__________
        #region Note_Mapped

        #endregion
    }

}
