using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FidoWithAspnetIdentity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;

namespace FidoWithAspnetIdentity.Controllers
{
    public class FidoController : Controller
    {
        private readonly IFidoAuthentication _fido;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<Registration> _logger;
        private readonly IEmailSender _emailSender;

        public FidoController(IFidoAuthentication fido, 
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<Registration> logger,
            IEmailSender emailSender)
        {
            _fido = fido;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        
        public IActionResult StartRegistration()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser {UserName = model.Email, Email = model.Email};
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new {area = "Identity", userId = user.Id, code = code, returnUrl = "~/"},
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new {email = model.Email, returnUrl = "~/"});
                    }
                    else
                    {
                        // initalise registration process
                        var challenge = await _fido.InitiateRegistration(model.Email, model.DeviceName);
                        // challenge the device
                        return View(challenge.ToBase64Dto());
                    }
                }

                if (result.Errors.Count() != 0)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create user");
                }
                
            }
            return View("StartRegistration", model);

        }
        
        [HttpPost]
        public async Task<IActionResult> CompleteRegistration(
            [FromQuery] string userName,
            [FromBody] Base64FidoRegistrationResponse registrationResponse)
        {
            var result = await _fido.CompleteRegistration(registrationResponse.ToFidoResponse());
            if (result.IsError)
            {
                var user = await _userManager.FindByEmailAsync(userName);
                var res = await _userManager.DeleteAsync(user);


                return BadRequest(result.ErrorDescription);
            }

            return Ok();
        }

        public async Task<IActionResult> Login()
        {
                var challenge = await _fido.InitiateAuthentication(null);
                
                return View(challenge.ToBase64Dto());
                
        }

        [HttpPost]
        public async Task<IActionResult> CompleteLogin(
            [FromBody] Base64FidoAuthenticationResponse authenticationResponse)
        {
            var authenticationResult = await _fido.CompleteAuthentication(authenticationResponse.ToFidoResponse());

            if (authenticationResult.IsSuccess)
            {
                var user = await _userManager.FindByEmailAsync(authenticationResult.UserId);

                
                await _signInManager.SignInAsync(user, false);
            }
            
            if (authenticationResult.IsError) return BadRequest(authenticationResult.ErrorDescription);

            return Ok();

        }
    }
}








