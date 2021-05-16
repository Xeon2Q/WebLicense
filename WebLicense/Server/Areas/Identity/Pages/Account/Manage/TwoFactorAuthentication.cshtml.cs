using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_TwoFactorAuthenticationModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanEnable2FA)]
    public class TwoFactorAuthenticationModel : PageModel
    {
        #region C-tor | Fields

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public TwoFactorAuthenticationModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        #endregion

        #region Properties

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
            Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
            IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<TwoFactorAuthenticationModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            await signInManager.ForgetTwoFactorClientAsync();

            logger.With(LogAction.ForgetBrowser2Fa, user).LogInformation(ResL.Message_2FA_BrowserHasForgotten);
            StatusMessage = new StatusMessageModel(ResL.Message_2FA_BrowserHasForgotten, true).ToJson();

            return RedirectToPage();
        }

        #endregion
    }
}