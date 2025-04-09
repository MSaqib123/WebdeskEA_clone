using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Core.Configuration
{

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }
        public int PermissionIndex { get; }

        public PermissionRequirement(string claimType, int permissionIndex)
        {
            ClaimType = claimType;
            PermissionIndex = permissionIndex;
        }
    }

}
