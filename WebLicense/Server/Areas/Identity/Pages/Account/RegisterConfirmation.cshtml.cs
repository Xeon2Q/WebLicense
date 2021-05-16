using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync([FromServices] UserManager<User> userManager, string email)
        {
            // email is not provided
            if (string.IsNullOrWhiteSpace(email)) return RedirectToPage("/Index");

            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return Page();

            // user not found
            return NotFound($"Unable to load user with email '{email}'.");
        }
    }
}
