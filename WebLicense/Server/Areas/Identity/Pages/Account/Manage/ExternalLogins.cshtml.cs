using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_ExternalLoginsModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanLoginExternal)]
    public class ExternalLoginsModel : PageModel
    {
        #region C-tor | Fields

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<ExternalLoginsModel> logger;

        public ExternalLoginsModel(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<ExternalLoginsModel> logger)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Properties

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            CurrentLogins = await userManager.GetLoginsAsync(user);
            OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).Where(q => CurrentLogins.All(w => q.Name != w.LoginProvider)).ToList();
            ShowRemoveButton = !string.IsNullOrWhiteSpace(user.PasswordHash) || CurrentLogins.Count > 1;

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                logger.LogErrorWith(LogAction.Account.Profile.RemoveExternalLogin, user, null, ResL.Log_LoginNotRemoved, loginProvider);
                StatusMessage = new StatusMessageModel(ResL.Message_LoginNotRemoved, false).ToJson();

                return RedirectToPage();
            }

            await signInManager.RefreshSignInAsync(user);

            logger.LogInformationWith(LogAction.Account.Profile.RemoveExternalLogin, user, ResL.Log_LoginRemoved, loginProvider);
            StatusMessage = new StatusMessageModel(ResL.Message_LoginNotRemoved, true).ToJson();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            var info = await signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                logger.LogErrorWith(LogAction.Account.Profile.LinkExternalLogin, user, null, ResL.Error_CannotLoadExternalLoginForUser, user.Id);

                throw new InvalidOperationException(string.Format(ResL.Error_CannotLoadExternalLoginForUser, user.Id));
            }

            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                logger.LogErrorWith(LogAction.Account.Profile.LinkExternalLogin, user, null, ResL.Log_LoginNotAdded, info.LoginProvider);
                StatusMessage = new StatusMessageModel(ResL.Message_LoginNotAdded, false).ToJson();

                return RedirectToPage();
            }

            // clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            logger.LogErrorWith(LogAction.Account.Profile.LinkExternalLogin, user, null, ResL.Log_LoginAdded, info.LoginProvider);
            StatusMessage = new StatusMessageModel(ResL.Message_LoginAdded, true).ToJson();

            return RedirectToPage();
        }

        #endregion
    }
}
