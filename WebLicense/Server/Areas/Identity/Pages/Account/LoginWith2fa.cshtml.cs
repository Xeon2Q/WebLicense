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
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_LoginWith2faModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [StringLength(7, MinimumLength = 6, ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_StringLength")]
            [DataType(DataType.Text)]
            [Display(Name = "Model_TwoFactorCode", ResourceType = typeof(ResL))]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Model_RememberMachine", ResourceType = typeof(ResL))]
            public bool RememberMachine { get; set; }
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync([FromServices] SignInManager<User> signInManager, bool rememberMe, string returnUrl = null)
        {
            // ensure the user has gone through the username & password screen first
            var _ = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException(ResL.Error1);

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<LoginWith2faModel> logger, bool rememberMe, string returnUrl = null)
        {
            // return with errors
            if (!ModelState.IsValid) return Page();

            returnUrl ??= Url.Content("~/");

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException(ResL.Error1);

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);
            if (result.Succeeded)
            {
                logger.LogInformationWith(LogAction.Account.Login.TwoFactor, user, ResL.Log1);

                return LocalRedirect(returnUrl);
            }
            
            if (result.IsLockedOut)
            {
                logger.LogWarningWith(LogAction.Account.Login.TwoFactor, user, ResL.Log2);

                return RedirectToPage("./Lockout");
            }

            // return with errors
            logger.LogWarningWith(LogAction.Account.Login.TwoFactor, user, ResL.Log3);
            ModelState.AddModelError(string.Empty, ResL.Error2);

            return Page();
        }

        #endregion
    }
}
