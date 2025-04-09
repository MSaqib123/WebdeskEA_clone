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
    public class UserRight:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int ModuleId { get; set; }
        [Required]

        public string UserId { get; set; }
        public bool RightInsert { get; set; }
        public bool RightUpdate { get; set; }
        public bool RightView { get; set; }
        public bool RightPrint { get; set; }
        public bool RightDelete { get; set; }
        //public bool RightEdit { get; set; }
        public bool RightApprove { get; set; }
        public bool RightEmail { get; set; }
        public bool RightNotification { get; set; }
        [NotMapped]
        public string ModuleName { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
