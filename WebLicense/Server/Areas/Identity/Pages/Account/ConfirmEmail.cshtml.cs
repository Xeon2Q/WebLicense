using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.PageModels;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ConfirmEmailModel;
using ResG = WebLicense.Server.Resources.Global;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        [TempData]
        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync([FromServices] UserManager<User> userManager, string userId = default, string code = default)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return LocalRedirect("/");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userId));
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);

            Status = result.Succeeded
                ? new StatusMessageModel(ResL.Message_ConfirmedSuccess, true).ToJson()
                : new StatusMessageModel(ResL.Message_ConfirmedError, false).ToJson();

            return Page();
        }
    }
}
