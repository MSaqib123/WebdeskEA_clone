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
    public class PackagePermission
    {

        //__________ Main Columns __________
        #region Main_Columns
        [Key]
        public int Id { get; set; } // Corresponds to [id] [int] IDENTITY(1,1) NOT NULL
        public int? PackageId { get; set; } // Corresponds to [PackageId] [int] NULL
        public int? ModuleId { get; set; } // Corresponds to [ModuleId] [int] NULL
        public bool? IsModuleActive { get; set; } // Corresponds to [isModuleActive] [bit] NULL
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        public object PackageName { get; set; }
        #endregion

        //__________ List and Single Object__________
        #region List
        #endregion

    }
}

