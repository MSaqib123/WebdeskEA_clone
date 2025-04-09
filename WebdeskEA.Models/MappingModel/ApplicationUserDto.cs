using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingModel
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto()
        {
            RoleDtos = new List<RoleDto>();
        }

        #region Main_Columns

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [ValidateNever]
        [EmailAddress]
        public string Email { get; set; } 
        public string? Name { get; set; } 
        public string? StreetAddress { get; set; }
        public string? City { get; set; } 
        public string? State { get; set; }
        public string? PostalCode { get; set; } 
        public string? ProfileImage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; } 
        public string? CurrentPassword { get; set; }
        public int? CompanyId { get; set; }
        public bool isTenantAdmin { get; set; }
        public int? TenantId { get; set; }
        public int? roleId { get; set; }

        [ValidateNever]
        public string? DisplayPassword { get; set; }
        public bool Active { get; set; }
        [ValidateNever]
        public string CreatedBy { get; set; }
        [ValidateNever]
        public DateTime CreatedOn { get; set; }
        [ValidateNever]
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        #endregion

        #region Mapped Columns
        [NotMapped]
        [ValidateNever]
        public IFormFile? ProfileImageForSave { get; set; } // File upload for profile image

        [NotMapped]
        [ValidateNever]
        public IEnumerable<RoleDto> RoleDtos { get; set; }
        #endregion

    }
}
