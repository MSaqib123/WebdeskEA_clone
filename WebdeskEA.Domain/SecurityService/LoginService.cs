using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.ViewModel;

namespace WebdeskEA.Domain.SecurityService
{
    public class LoginService : ILoginService
    {
        #region Constructor
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(SignInManager<ApplicationUser> signInManager,
                            UserManager<ApplicationUser> userManager,
                            ILogger<LoginService> logger,
                            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Main _ Method
        public async Task<IActionResult> LoginAsync(LoginInputViewModel input, string returnUrl)
        {
            // Clear all session data
            _httpContextAccessor.HttpContext.Session.Clear();

            returnUrl ??= "~/";

            if (IsSuperAdmin(input.Email, input.Password))
            {
                return await SignInSuperAdminAsync(returnUrl);
            }
            var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                // Option A: ALWAYS go back to the returnUrl if it's safe
                if (IsLocalUrl(returnUrl))
                {
                    return new LocalRedirectResult(returnUrl);
                }
                else
                {
                    // Fallback if the user came from an external site or no returnUrl was provided
                    return new LocalRedirectResult("/Dashboard/DSB_Home/Index");
                }
            }
            else if (result.RequiresTwoFactor)
            {
                return new RedirectToActionResult("LoginWith2fa", "Security", new { ReturnUrl = returnUrl, RememberMe = input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return new RedirectToActionResult("Lockout", "Security", null);
            }
            else
            {
                throw new Exception("Invalid login attempt.");
            }
        }
        #endregion

        #region internal method
        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return url.StartsWith("/") && !url.StartsWith("//") && !url.StartsWith("/\\");
        }
        private bool IsSuperAdmin(string email, string password)
        {
            return email == "SuperAdmin@gmail.com" && password == "admin.9090";
        }

        private async Task<IActionResult> SignInSuperAdminAsync(string returnUrl)
        {
            _logger.LogInformation("SuperAdmin logged in.");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "SuperAdmin@gmail.com"),
            new Claim(ClaimTypes.Role, "SuperAdmin")
        };

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            return new LocalRedirectResult("/Dashboard/DSB_SuperAdmin/Index");
        }
        #endregion
        
    }
}

