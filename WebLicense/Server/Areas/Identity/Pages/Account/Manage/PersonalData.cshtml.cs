using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using ResG = WebLicense.Server.Resources.Global;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        #region Actions

        public async Task<IActionResult> OnGetAsync([FromServices] UserManager<User> userManager)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            return Page();
        }

        #endregion
    }
}