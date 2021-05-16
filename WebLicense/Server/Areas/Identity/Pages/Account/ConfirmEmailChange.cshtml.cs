using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.PageModels;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ConfirmEmailChangeModel;
using ResG = WebLicense.Server.Resources.Global;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ConfirmEmailChangeModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userId));
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                Status = new StatusMessageModel(ResL.Message_ChangeEmailFailed, false).ToJson();
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            Status = new StatusMessageModel(ResL.Message_ChangeEmailSuccess, true).ToJson();

            return Page();
        }
    }
}
