using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.Utility.AttributeUtality
{
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _policyName;

        public CustomAuthorizeAttribute(string policyName)
        {
            _policyName = policyName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var policyProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();

            // Check if user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Redirect to the login page
                context.Result = new ChallengeResult();
                return;
            }

            // Check if the policy exists
            var policy = await policyProvider.GetPolicyAsync(_policyName);
            if (policy == null)
            {
                throw new UnauthorizedAccessException("Policy does not exist.");
            }

            // Check if the user meets the policy requirements
            var authResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, _policyName);
            if (!authResult.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
