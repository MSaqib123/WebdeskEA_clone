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

    public class Brand : BaseEntity
    {
        //__________ Main Columns __________
        #region Main_Columns 
        public int Id { get; set; } // Primary Key
        public string BrandName { get; set; }
        public int CompanyId { get; set; }
        public int TenantId { get; set; }
        #endregion


        //------ Joined Columns -----
        #region Joined_Columns
        #endregion

        //__________ List and Single Object__________
        #region Note_Mapped
        #endregion
    }

}
