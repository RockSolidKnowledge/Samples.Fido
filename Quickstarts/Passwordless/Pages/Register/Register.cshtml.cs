using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;

namespace Core.Pages.Register;

public class RegisterModel : PageModel
{
    private readonly IFidoAuthentication _fidoAuthentication;

    [BindProperty(SupportsGet = true)]
    public Base64FidoRegistrationChallenge? FidoChallenge { get; set; }
    

    public RegisterModel(IFidoAuthentication _fidoAuthentication)
    {
        this._fidoAuthentication = _fidoAuthentication;
    }
    
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost([FromBody]Base64FidoRegistrationResponse response)
    {
        var result = await _fidoAuthentication.CompleteRegistration(response.ToFidoResponse());

        if (result.IsError)
        {
            return BadRequest(result.ErrorDescription);
        }

        return new EmptyResult();
    }
}