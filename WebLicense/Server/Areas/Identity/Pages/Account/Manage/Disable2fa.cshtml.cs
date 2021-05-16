using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_Disable2faModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanDisable2FA)]
    public class Disable2faModel : PageModel
    {
        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public Disable2faModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
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
            if (!user.TwoFactorEnabled) throw new InvalidOperationException(string.Format(ResL.Validation_2FANotEnabled, user.Id));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<Disable2faModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            var result = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                logger.With(LogAction.Disable2FA, user).LogError(string.Format(ResL.Error_Unexpected, user.Id));

                throw new InvalidOperationException(string.Format(ResL.Error_Unexpected, user.Id));
            }

            logger.With(LogAction.Disable2FA, user).LogInformation(string.Format(ResL.Message_2FADisableSuccessful, user.Id));

            StatusMessage = new StatusMessageModel(ResL.Message_2FADisabledSuccessful_2, true).ToJson();

            return RedirectToPage("./TwoFactorAuthentication");
        }

        #endregion
    }
}