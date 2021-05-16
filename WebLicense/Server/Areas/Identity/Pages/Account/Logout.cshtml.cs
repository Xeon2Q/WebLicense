using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPost([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<LogoutModel> logger, string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            
            logger.LogInformation("User logged out.");

            return returnUrl != null
                ? LocalRedirect(returnUrl)
                : RedirectToPage();
        }
    }
}
