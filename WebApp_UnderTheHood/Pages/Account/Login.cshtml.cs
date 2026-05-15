using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp_UnderTheHood.Authorization;

namespace WebApp_UnderTheHood.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public Credential Credential {  get; set; }  = new Credential();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        if(Credential.UserName == "admin" && Credential.Password == "admin")
        {
            //Crating the security context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Credential.UserName),
                new Claim(ClaimTypes.Email,"admin@thisNewSite"),
                new Claim("Deparment", "HR"),
                new Claim("Admin", "true"),
                new Claim("Manager","true"),
                new Claim("EmploymentDate", "2025-04-01")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, new()
            {
                IsPersistent = Credential.RememberMe
            });

            return RedirectToPage("/Index");
        }

        return Page();
    }
}
