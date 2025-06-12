using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;

namespace Core.Pages.Login;

public class IndexModel : PageModel
{
    private readonly IFidoAuthentication _fidoAuthentication;

    [BindProperty]
    public string? UserId { get; set; }

    public IndexModel(IFidoAuthentication fidoAuthentication)
    {
        _fidoAuthentication = fidoAuthentication;
    }

    public IActionResult OnGet()
    {
        return Page();
    }
    
    public async Task<IActionResult> OnPost()
    {
        try
        {
            var challenge = await _fidoAuthentication.InitiateAuthentication(UserId);

            var dto = challenge.ToBase64Dto();

            return RedirectToPage("Login", new
            {
                dto.UserId,
                dto.Base64Challenge,
                dto.RelyingPartyId,
                dto.Base64KeyIds,

            });
        }
        catch(PublicKeyCredentialException exception)
        {
            return BadRequest(exception.Message);
        }
        
    }
}