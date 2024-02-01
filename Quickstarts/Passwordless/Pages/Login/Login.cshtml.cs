using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;

namespace Core.Pages.Login;

public class LoginModel : PageModel
{
    private readonly IFidoAuthentication _fidoAuthentication;

    [BindProperty(SupportsGet = true)]
    public Base64FidoAuthenticationChallenge Challenge { get; set; }

    public LoginModel(IFidoAuthentication fidoAuthentication)
    {
        _fidoAuthentication = fidoAuthentication;
    }
    
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost([FromBody] Base64FidoAuthenticationResponse authenticationResponse)
    {
        var result = await _fidoAuthentication.CompleteAuthentication(authenticationResponse.ToFidoResponse());

        if (result.IsSuccess)
        {
            await HttpContext.SignInAsync("cookie", new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("sub", result.UserId)
            }, "cookie")));
        }

        if (result.IsError) return BadRequest(result.ErrorDescription);
        
        return RedirectToPage("/Index");
    }
}