using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_GenerateRecoveryCodesModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanEnable2FA)]
    public class GenerateRecoveryCodesModel : PageModel
    {
        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public GenerateRecoveryCodesModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region Properties

        [TempData]
        public string[] RecoveryCodes { get; set; }

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

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<GenerateRecoveryCodesModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));
            if (!user.TwoFactorEnabled) throw new InvalidOperationException(string.Format(ResL.Validation_2FANotEnabled, user.Id));

            RecoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToArray();

            logger.LogInformationWith(LogAction.Account.Profile.GenerateRecoveryCodes2FA, user, ResL.Log_CodesGenerated, user.Id);

            StatusMessage = new StatusMessageModel(ResL.Message_CodesGenerated, true).ToJson();

            return RedirectToPage("./ShowRecoveryCodes");
        }

        #endregion
    }
}