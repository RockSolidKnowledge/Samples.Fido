using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FidoWithAspnetIdentity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;

namespace FidoWithAspnetIdentity.Controllers
{
    public class FidoController : Controller
    {
        private readonly IFidoAuthentication _fido;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public FidoController(IFidoAuthentication fido, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _fido = fido;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult StartRegistration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DeviceRegistration model)
        {
            // initalise registration process
            var challenge = await _fido.InitiateRegistration(User.Identity.Name, model.DeviceName);
            // challenge the device
            return View(challenge.ToBase64Dto());
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration(
            [FromBody] Base64UrlFidoRegistrationResponse registrationResponse)
        {
            var result = await _fido.CompleteRegistration(registrationResponse.ToFidoResponse());

            if (result.IsError) return BadRequest(result.ErrorDescription);
            return Ok();
        }

        public async Task<IActionResult> Login()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
            if (result.Succeeded)
            {
                var claims = result.Principal.Claims.ToList();
                string userName = claims.FirstOrDefault(c => c.Type == "userName")?.Value;
                var challenge = await _fido.InitiateAuthentication(userName);

                return View(challenge.ToBase64Dto());
            }

            return new RedirectResult("/Home/Error");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteLogin(
            [FromBody] Base64UrlFidoAuthenticationResponse authenticationResponse)
        {
            var authenticationResult = await _fido.CompleteAuthentication(authenticationResponse.ToFidoResponse());

            if (authenticationResult.IsSuccess)
            {
                var result = await HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);

                var claims = result.Principal.Claims.ToList();
                string rememberMeClaim = claims.FirstOrDefault(c => c.Type == "rememberme")?.Value;
                bool rememberMe = bool.Parse(rememberMeClaim ?? "false");
                string userName = claims.FirstOrDefault(c => c.Type == "userName")?.Value;
                string password = claims.FirstOrDefault(c => c.Type == "password")?.Value;

                await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);
            }

            await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

            if (authenticationResult.IsError) return BadRequest(authenticationResult.ErrorDescription);

            return Ok();
        }
    }
}