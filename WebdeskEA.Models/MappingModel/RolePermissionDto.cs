using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.DbModel;

namespace WebdeskEA.Models.MappingModel
{
    public class RolePermissionDto
    {
        public RolePermissionDto()
        {
            roleDto = new RoleDto();
            moduleDtos = new List<ModuleDto>();
            rolePermission = new List<RolePermissionDto>();
        }
        //------ Main Columns -----
        #region Main_Columns
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
        [ValidateNever]
        public string? UserId { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? UserName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? TenantName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? CompanyName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? RoleName { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? ModuleName { get; set; }
        [NotMapped]
        [ValidateNever]
        public int TenantUsers { get; set; }
        [NotMapped]
        [ValidateNever]
        public int TenantCompanies { get; set; }
        #endregion

        //------ Not Mapped -----
        #region Not_Mapped
        [NotMapped]
        public RoleDto roleDto { get; set; }
        [NotMapped]
        public IEnumerable<ModuleDto> moduleDtos { get; set; }
        [NotMapped]
        public IEnumerable<RolePermissionDto>  rolePermission { get; set; }
        #endregion
    }

}
