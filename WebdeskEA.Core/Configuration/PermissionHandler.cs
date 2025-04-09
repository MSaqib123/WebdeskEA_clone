using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Core.Configuration
{

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Handle role-based authorization
            if (context.User.IsInRole(requirement.ClaimType))
            {
                context.Succeed(requirement);
            }

            // Handle permission-based authorization
            var permissionClaim = context.User.FindFirst(c => c.Type == requirement.ClaimType);
            if (permissionClaim != null && int.TryParse(permissionClaim.Value, out int packedPermissions))
            {
                if ((packedPermissions & (1 << requirement.PermissionIndex)) != 0)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
