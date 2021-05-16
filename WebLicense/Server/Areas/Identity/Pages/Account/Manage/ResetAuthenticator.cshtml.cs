using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_ResetAuthenticatorModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanEnable2FA)]
    public class ResetAuthenticatorModel : PageModel
    {
        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public ResetAuthenticatorModel(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        #region Properties

        [TempData]
        public string StatusMessage { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<ResetAuthenticatorModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // disable 2fa & refresh signin
            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            await signInManager.RefreshSignInAsync(user);
            
            logger.LogInformation(string.Format(ResL.Format_UserDisabledTwoFactorAuthentication, user.Id));
            StatusMessage = new StatusMessageModel(ResL.Message_AuthenticatorWasReset, true).ToJson();

            return RedirectToPage("./EnableAuthenticator");
        }

        #endregion
    }
}