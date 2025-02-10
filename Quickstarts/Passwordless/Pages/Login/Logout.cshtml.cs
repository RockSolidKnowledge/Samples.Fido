using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Core.Pages.Login;

public class Logout : PageModel
{
    public async Task OnGet()
    {
        await HttpContext.SignOutAsync("cookie");
    }
}