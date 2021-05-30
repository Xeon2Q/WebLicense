using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_LoginWithRecoveryCodeModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Model_RecoveryCode", ResourceType = typeof(ResL))]
            public string RecoveryCode { get; set; }
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync([FromServices] SignInManager<User> signInManager, string returnUrl = null)
        {
            // ensure the user has gone through the username & password screen first
            var _ = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException(ResL.Error1);

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<LoginWithRecoveryCodeModel> logger, string returnUrl = null)
        {
            // return with errors
            if (!ModelState.IsValid) return Page();

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException(ResL.Error1);

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                logger.LogInformationWith(LogAction.Account.Login.RecoveryCode, user, ResL.Log_1);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }

            if (result.IsLockedOut)
            {
                logger.LogWarningWith(LogAction.Account.Login.RecoveryCode, user, ResL.Log_2);
                return RedirectToPage("./Lockout");
            }

            logger.LogWarningWith(LogAction.Account.Login.RecoveryCode, user, ResL.Log_3);
            ModelState.AddModelError(string.Empty, ResL.Validation_InvalidRecoveryCode);
            return Page();
        }

        #endregion
    }
}
