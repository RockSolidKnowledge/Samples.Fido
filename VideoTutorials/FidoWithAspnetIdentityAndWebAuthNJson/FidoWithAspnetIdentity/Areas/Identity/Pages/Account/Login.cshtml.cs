using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rsk.AspNetCore.Fido.Stores;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;



namespace FidoWithAspnetIdentity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IFidoKeyStore _keyStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IFidoKeyStore keyStore, SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger)
        {
            _keyStore = keyStore;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
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

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var (logonResult, signInResult) = await DoLogon(returnUrl);

                if (!signInResult.Succeeded)
                {
                    return logonResult;
                }

                var ids = (await _keyStore.GetCredentialIdsForUser(Input.Email))?.ToList();

                if (ids == null || ids.Count == 0)
                {
                    return logonResult;
                }
                else
                {
                    await _signInManager.SignOutAsync();

                    await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(
                            BuildClaims(Input.Email, Input.Password, Input.RememberMe),
                            IdentityConstants.TwoFactorUserIdScheme)));
                    
                    return Redirect("/Fido/Login");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        
        private IEnumerable<Claim> BuildClaims(string userName, string password, bool rememberme)
        {
            var claims = new List<Claim>();
            
            claims.Add(new Claim("userName", userName));
            claims.Add(new Claim("password", password));
            claims.Add(new Claim("rememberme", rememberme.ToString()));

            return claims;
        }
        
        private async Task<(IActionResult actionResult, SignInResult signInResult)> DoLogon(string returnUrl)
        {
            IActionResult returnResult;
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result =
                await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe,
                    lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return (LocalRedirect(returnUrl), result);
            }

            if (result.RequiresTwoFactor)
            {
                return (RedirectToPage("/LoginWith2fa", new {ReturnUrl = returnUrl, RememberMe = Input.RememberMe}),
                    result);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                returnResult = RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                returnResult = Page();
            }

            return (returnResult, result);
        }

    }
}
