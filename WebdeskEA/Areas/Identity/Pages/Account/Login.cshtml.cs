// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebdeskEA.Models.ExternalModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebdeskEA.Domain.RepositoryDapper.IRepository;
using WebdeskEA.Models.Utility;
using WebdeskEA.Domain.SecurityService;
using WebdeskEA.Models.ViewModel;
using Microsoft.Identity.Client;

namespace WebdeskEA.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILogger<LoginModel> _logger;
        private readonly ILoginService _loginService;

        public LoginModel(
            ILoginService loginService,
            ITenantRepository tenantRepository,
            IErrorLogRepository errorLogRepository, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, 
            ILogger<LoginModel> logger)
        {
            _loginService = loginService;
            _tenantRepository = tenantRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _errorLogRepository = errorLogRepository ?? throw new ArgumentNullException(nameof(errorLogRepository));
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        // You may still want this for clearing external cookies, setting up external logins, etc.
        public async Task OnGetAsync(string returnUrl = null)
        {
            // If there was an error, display it
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Example: Clear external cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // 1. If model isn't valid, just show the page again
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                //====== Login Process =====
                var loginInputModel = new LoginInputViewModel
                {
                    Email = Input.Email,
                    Password = Input.Password,
                    RememberMe = Input.RememberMe
                };
                // 2. Call the LoginService
                IActionResult result = await _loginService.LoginAsync(loginInputModel, ReturnUrl);

                // 3. Check if the service returned an error
                if (result is BadRequestObjectResult)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                // 4. Otherwise, the service gave us a redirect (to /Dashboard/... or /Security/Lockout, etc.)
                return result;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Login Failed!";
                await LogError(ex); // your custom logging, if desired
                return Page();
            }
        }


        //=================== Error Logs =====================
        #region ErrorHandling
        private async Task LogError(Exception ex)
        {
            var (errorCode, statusCode) = ErrorUtility.GenerateErrorCodeAndStatus(ex);

            //string areaName = ControllerContext.RouteData.Values["area"]?.ToString() ?? "Area Not Found";
            //string controllerName = ControllerContext.RouteData.Values["controller"]?.ToString() ?? "Controler Not Found";
            //string actionName = ControllerContext.RouteData.Values["action"]?.ToString() ?? "ActionName not Found";

            await _errorLogRepository.AddErrorLogAsync(
                area: "Identity",
                controller: "LoginPage",
                actionName: "Login",
                formName: $"Login Form",
                errorShortDescription: ex.Message,
                errorLongDescription: ErrorUtility.GetFullExceptionMessage(ex),
                statusCode: statusCode.ToString(),
                username: User.Identity?.Name ?? "GuestUser"
            );
        }
        #endregion




        //    public async Task OnGetAsync(string returnUrl = null)
        //    {
        //        if (!string.IsNullOrEmpty(ErrorMessage))
        //        {
        //            ModelState.AddModelError(string.Empty, ErrorMessage);
        //        }

        //        returnUrl ??= Url.Content("~/");

        //        // Clear the existing external cookie to ensure a clean login process
        //        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //        ReturnUrl = returnUrl;
        //    }


        //    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //    {
        //        try
        //        {
        //            // Clear all session data
        //            HttpContext.Session.Clear();

        //            returnUrl ??= Url.Content("~/");

        //            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //            if (ModelState.IsValid)
        //            {
        //                // Handle the hardcoded SuperAdmin login
        //                if (IsSuperAdmin(Input.Email, Input.Password))
        //                {
        //                    return await SignInSuperAdmin(returnUrl);
        //                }

        //                // Handle normal login
        //                return await HandleNormalLogin(returnUrl);
        //            }

        //            return Page();
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = "Login Faild!";
        //            await LogError(ex);
        //            return Page();
        //        }
        //    }

        //    private bool IsSuperAdmin(string email, string password)
        //    {
        //        return email == "SuperAdmin@gmail.com" && password == "admin.9090";
        //    }

        //    private async Task<IActionResult> SignInSuperAdmin(string returnUrl)
        //    {
        //        _logger.LogInformation("SuperAdmin logged in.");

        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, "SuperAdmin@gmail.com"),
        //    new Claim(ClaimTypes.Role, "SuperAdmin")
        //};

        //        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        //        var principal = new ClaimsPrincipal(identity);

        //        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        //        return LocalRedirect("/Dashboard/DSB_SuperAdmin/Index");
        //    }

        //    private async Task<IActionResult> HandleNormalLogin(string returnUrl)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User logged in.");
        //            return LocalRedirect("/Dashboard/DSB_Home/Index");
        //        }
        //        else if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //        }
        //        else if (result.IsLockedOut)
        //        {
        //            TempData["Error"] = "User account locked out.";
        //            return RedirectToPage("./Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            TempData["Error"] = "Invalid login attempt!";
        //            return Page();
        //        }
        //    }



        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");

        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    if (ModelState.IsValid)
        //    {
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation("User logged in.");
        //            return LocalRedirect(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning("User account locked out.");
        //            return RedirectToPage("./Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return Page();
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return Page();
        //}

    }
}
