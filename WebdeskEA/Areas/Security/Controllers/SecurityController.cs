using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebdeskEA.Areas.Identity.Pages.Account;
using WebdeskEA.Models.ExternalModel;

namespace WebdeskEA.Areas.Security.Controllers
{
    public class SecurityController : Controller
    {
        #region Custructor
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<SecurityController> logger,
            IUserStore<ApplicationUser> userStore,
            IEmailSender emailSender
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
        }
        #endregion

        //**************************************************************************
        //*************************** Helpers Section ******************************
        //**************************************************************************
        #region Helpers

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        #endregion

        //**************************************************************************
        //*************************** Login Section ********************************
        //**************************************************************************
        #region  Login___Section
        //==============================================================
        //================== Section 1 : Login =========================
        //==============================================================
        #region Login Section

        // Model for login input
        public class LoginInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ViewData["ExternalLogins"] = externalLogins;
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel input, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Handle the hardcoded SuperAdmin login
                if (IsSuperAdmin(input.Email, input.Password))
                {
                    return await SignInSuperAdmin(returnUrl);
                }

                // Handle normal login
                return await HandleNormalLogin(input, returnUrl);
            }

            return View();
        }

        private bool IsSuperAdmin(string email, string password)
        {
            return email == "SuperAdmin@gmail.com" && password == "admin.9090";
        }

        private async Task<IActionResult> SignInSuperAdmin(string returnUrl)
        {
            _logger.LogInformation("SuperAdmin logged in.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "SuperAdmin@gmail.com"),
                new Claim(ClaimTypes.Role, "SuperAdmin")
            };

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            return LocalRedirect("/Dashboard/DSB_SuperAdmin/Index");
        }

        private async Task<IActionResult> HandleNormalLogin(LoginInputModel input, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                // Get the current user
                var user = await _userManager.FindByEmailAsync(input.Email);

                // Add claims for user identifier and username
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Add User ID
                    new Claim(ClaimTypes.Name, user.UserName) // Add UserName
                };

                // Add Admin role claim if necessary
                // if (await _userManager.IsInRoleAsync(user, "Admin"))
                //{
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                //}

                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                return LocalRedirect("/Dashboard/DSB_Home/Index");
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
        }

        #endregion

        //==============================================================
        //================== Section 2 : LoginWith2fa ==================
        //==============================================================
        #region LoginWith2fa
        public class LoginWith2faViewModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }

            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            returnUrl ??= Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToAction("Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View(model);
            }
        }

        #endregion


        //==============================================================
        //================== Section 3 : LoginWithRecoveryCode =========
        //==============================================================
        #region LoginWithRecoveryCode
        public class LoginWithRecoveryCodeViewModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }

            public string ReturnUrl { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            var model = new LoginWithRecoveryCodeViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction("Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View(model);
            }
        }

        #endregion

        //==============================================================
        //================== Section 4 : Logout ========================
        //==============================================================
        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Redirecting to homepage or login after logout
                return RedirectToAction("Index", "Home"); // Or redirect to Login page
            }
        }

        #endregion

        //==============================================================
        //================== Section 5 : Lockout =======================
        //==============================================================
        #region Lockout

        [HttpGet]
        public IActionResult Lockout()
        {
            // Render the Lockout view
            return View();
        }

        #endregion


        //==============================================================
        //================= Section 6 : ExternalLogin ==================
        //==============================================================
        #region External Login

        public class ExternalLoginViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public string ReturnUrl { get; set; }
            public string ProviderDisplayName { get; set; }
        }


        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Security", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", new { returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToAction("Login", new { returnUrl });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                var email = info.Principal.HasClaim(c => c.Type == ClaimTypes.Email) ? info.Principal.FindFirstValue(ClaimTypes.Email) : null;
                ViewBag.Email = email;
                ViewBag.ProviderDisplayName = info.ProviderDisplayName;
                return View("ExternalLoginConfirmation", new ExternalLoginViewModel { ReturnUrl = returnUrl });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    TempData["ErrorMessage"] = "Error loading external login information during confirmation.";
                    return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Security", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.ProviderDisplayName = model.ProviderDisplayName;
            return View("ExternalLoginConfirmation", model);
        }

        #endregion

        //==============================================================
        //================= Section 7 : ForgotPassword ==================
        //==============================================================
        #region ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action("ResetPassword", "Security", new { code }, Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        public class ForgotPasswordInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        #endregion

        //==============================================================
        //================= Section 8 : ForgotPasswordConfirmation =====
        //==============================================================
        #region ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion

        //==============================================================
        //================= Section 9 : Confirm Email ==================
        //==============================================================
        #region ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            ViewData["StatusMessage"] = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return View();
        }
        #endregion

        //==============================================================
        //================= Section 10 : ConfirmEmailChange ============
        //==============================================================
        #region ConfirmEmailChange
        // Existing methods for ForgotPassword and ConfirmEmail...

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                ViewData["StatusMessage"] = "Error changing email.";
                return View();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                ViewData["StatusMessage"] = "Error changing user name.";
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewData["StatusMessage"] = "Thank you for confirming your email change.";
            return View();
        }
        #endregion

        //==============================================================
        //================= Section 11 : ResendEmail Confirmation ======
        //==============================================================
        #region 
        // Existing RegisterConfirmation action

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return View(input);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = userId, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return View(input);
        }

        public class ResendEmailModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        #endregion

        //==============================================================
        //================= Section 12 : Reset Password ================
        //==============================================================
        #region ResetPassword
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var model = new ResetPasswordModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, input.Code, input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(input);
        }

        public class ResetPasswordModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }
        #endregion

        //==============================================================
        //================= Section 13 : ResetPassword Confirm =========
        //==============================================================
        #region ResetPassword confirm
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion

        #endregion

        //**************************************************************************
        //*************************** Register Section *****************************
        //**************************************************************************
        #region Register___Section

        //==============================================================
        //================== Section 1 : Register ======================
        //==============================================================
        #region Register
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(InputModel input, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateApplicationUser();
                await _userStore.SetUserNameAsync(user, input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Security",
                        new { userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(input);
        }

        private ApplicationUser CreateApplicationUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        #endregion

        //==============================================================
        //================== Section 2 : RegisterConfirmation ==========
        //==============================================================
        #region RegisterConfirmation
        [HttpGet]
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect to home if email is null
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            // Prepare view model data
            var viewModel = new RegisterConfirmationViewModel
            {
                Email = email,
                DisplayConfirmAccountLink = true,
                EmailConfirmationUrl = GenerateEmailConfirmationUrl(user, returnUrl)
            };

            return View(viewModel); // Return the view with the model
        }

        private string GenerateEmailConfirmationUrl(ApplicationUser user, string returnUrl)
        {
            var userId = _userManager.GetUserIdAsync(user).Result;
            var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            return Url.Action(
                "ConfirmEmail",
                "Account",
                new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);
        }

        public class RegisterConfirmationViewModel
        {
            public string Email { get; set; }
            public bool DisplayConfirmAccountLink { get; set; }
            public string EmailConfirmationUrl { get; set; }
        }
        #endregion

        #endregion


        //**************************************************************************
        //*************************** Helpers Section ******************************
        //**************************************************************************
    }
}
