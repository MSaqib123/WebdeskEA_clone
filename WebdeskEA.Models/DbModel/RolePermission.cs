using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.DbModel
{
    public class RolePermission
    {
        //------ Main Columns -----
        #region Main_Columns
        [Key]
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? ModuleId { get; set; }
        public bool? PermInsert { get; set; }
        public bool? PermUpdate { get; set; }
        public bool? PermView { get; set; }
        public bool? PermPrint { get; set; }
        public bool? PermDelete { get; set; }
        public bool? PermEmail { get; set; }
        public bool? PermNotification { get; set; }
        #endregion

        //------ Joined Columns -----
        #region Joined_Columns
        [NotMapped]
        public string? UserId { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? TenantName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? RoleName { get; set; }
        [NotMapped]
        public string? ModuleName { get; set; }
        [NotMapped]
        public int TenantUsers { get; set; }
        [NotMapped]
        public int TenantCompanies { get; set; }
        #endregion
    }

}
