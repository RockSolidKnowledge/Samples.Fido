using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;

namespace Core.Pages.Register;

public class IndexModel : PageModel
{
    private readonly IFidoAuthentication _fidoAuthentication;

    [BindProperty]
    public string UserId { get; set; }
    
    [BindProperty]
    public string DeviceName { get; set; }

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
        var challenge = await _fidoAuthentication.InitiateRegistration(UserId, DeviceName);

        var dto = challenge.ToBase64Dto();
        
        return RedirectToPage("Register", new
        {
            challenge.UserId, 
            dto.Base64UserHandle,
            dto.Base64Challenge,
            dto.DeviceFriendlyName,
            dto.Base64ExcludedKeyIds,
            dto.RelyingPartyId
        });    
    }
}