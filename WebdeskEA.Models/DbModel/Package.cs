using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.DbModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using WebdeskEA.Models.BaseEntites;

    public class Package : BaseEntity
    {
        //------ Main Columns -----
        #region Main_Columns
        [Key]
        public int Id { get; set; } 
        public int? PackageTypeId { get; set; } 
        public string PackageName { get; set; } 
        public int? TotalCompany { get; set; } 
        public int? TotalUser { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        public string PackageTypeName { get; set; }
        #endregion
    }
}
