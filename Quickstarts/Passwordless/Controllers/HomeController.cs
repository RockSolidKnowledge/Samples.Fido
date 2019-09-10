using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Passwordless.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Models;

namespace Passwordless.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFidoAuthentication fido;

        public HomeController(IFidoAuthentication fido)
        {
            this.fido = fido ?? throw new ArgumentNullException(nameof(fido));
        }

        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Secure() => View("Index");

        public IActionResult StartRegistration() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            var challenge = await fido.InitiateRegistration(model.UserId, model.DeviceName);

            return View(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration([FromBody] FidoRegistrationResponse registrationResponse)
        {
            var result = await fido.CompleteRegistration(registrationResponse);

            if (result.IsError) return BadRequest(result.ErrorDescription);
            return Ok();
        }

        public async Task<IActionResult> Login()
        {
            var challenge = await fido.InitiateAuthentication(null);

            return View(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteLogin([FromBody] FidoAuthenticationResponse authenticationResponse)
        {
            var result = await fido.CompleteAuthentication(authenticationResponse);

            if (result.IsSuccess)
            {
                await HttpContext.SignInAsync("cookie", new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim("sub", result.UserId)
                }, "cookie")));
            }

            if (result.IsError) return BadRequest(result.ErrorDescription);
            return Ok();
        }
    }
}
