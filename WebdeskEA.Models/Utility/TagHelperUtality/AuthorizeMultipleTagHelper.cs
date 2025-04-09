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
    [HtmlTargetElement("authorize-multiple", Attributes = "policies")]
    public class AuthorizeMultipleTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizeMultipleTagHelper(IAuthorizationService authorizationService,
                                          IAuthorizationPolicyProvider policyProvider,
                                          IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _policyProvider = policyProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("policies")]
        public string Policies { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var policies = Policies.Split(',');

            bool isAuthorized = false;

            foreach (var policyName in policies)
            {
                var policy = await _policyProvider.GetPolicyAsync(policyName.Trim());
                if (policy != null)
                {
                    var authResult = await _authorizationService.AuthorizeAsync(user, policyName.Trim());
                    if (authResult.Succeeded)
                    {
                        isAuthorized = true;
                        break;
                    }
                }
            }

            if (!isAuthorized)
            {
                output.SuppressOutput();
            }
        }
    }
}
