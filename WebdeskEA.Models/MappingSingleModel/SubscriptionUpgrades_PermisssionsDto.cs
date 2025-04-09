using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.MappingSingleModel
{
    public class SubscriptionUpgrades_PermisssionsDto
    {
        [ValidateNever]
        public int TotalCompany { get; set; }
        [ValidateNever]
        public int TotalUser { get; set; }
        [ValidateNever]
        public int PackageId { get; set; }
        [ValidateNever]
        public string PackageName { get; set; }
        [ValidateNever]
        public string RoleName { get; set; }
        [ValidateNever]
        public string ModuleName { get; set; }
        [ValidateNever]
        public string Permissions { get; set; }
    }
}
