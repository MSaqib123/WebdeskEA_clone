using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.Utility.TagHelperUtality
{
    [HtmlTargetElement("authorize", Attributes = "policy")]
    public class AuthorizeTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizeTagHelper(IAuthorizationService authorizationService,
                                  IAuthorizationPolicyProvider policyProvider,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _policyProvider = policyProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("policy")]
        public string Policy { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Get the current user
            var user = _httpContextAccessor.HttpContext.User;

            // Check if the policy exists
            var policy = await _policyProvider.GetPolicyAsync(Policy);
            if (policy == null)
            {
                // If policy doesn't exist, suppress the output
                output.SuppressOutput();
                return;
            }

            // If policy exists, perform the authorization check
            var authResult = await _authorizationService.AuthorizeAsync(user, Policy);
            if (!authResult.Succeeded)
            {
                // If authorization fails, suppress the output
                output.SuppressOutput();
            }
        }
    }
}
