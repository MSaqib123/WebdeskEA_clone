using WebdeskEA.Models.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;

namespace WebdeskEA.Models.ExternalModel
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? ProfileImage { get; set; }

        [ValidateNever]
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public virtual Company? Company { get; set; }

        public bool isTenantAdmin { get; set; }
        public int? roleId { get; set; }
        public int? TenantId { get; set; }
        [ValidateNever]
        public string? DisplayPassword { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }



        //____ NotMapped Properteis ______
        [NotMapped]
        public string Role { get; set; }
    }
}
