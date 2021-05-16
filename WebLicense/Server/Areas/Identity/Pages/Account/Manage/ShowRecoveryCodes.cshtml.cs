using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebLicense.Shared.Auxiliary.Policies;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanEnable2FA)]
    public class ShowRecoveryCodesModel : PageModel
    {
        #region Properties

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        #endregion

        #region Actions

        public IActionResult OnGet()
        {
            return RecoveryCodes == null || RecoveryCodes.Length == 0 || RecoveryCodes.Length % 2 != 0
                ? RedirectToPage("./TwoFactorAuthentication")
                : Page();
        }

        #endregion
    }
}
