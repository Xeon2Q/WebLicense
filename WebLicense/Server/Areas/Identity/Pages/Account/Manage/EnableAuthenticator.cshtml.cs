using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_EnableAuthenticatorModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.OwnAccount.Names.CanEnable2FA)]
    public class EnableAuthenticatorModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Model_Code", ResourceType = typeof(ResL))]
            [StringLength(7, MinimumLength = 6, ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_StringLength")]
            public string Code { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UserManager<User> userManager;
        private readonly UrlEncoder urlEncoder;

        public EnableAuthenticatorModel(UserManager<User> userManager, UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.urlEncoder = urlEncoder;
        }

        #endregion

        #region Properties

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            await InitializeModel(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<EnableAuthenticatorModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // return with errors
            if (!ModelState.IsValid)
            {
                await InitializeModel(user);
                return Page();
            }

            // strip spaces and hyphens
            var token = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var valid = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, token);

            if (!valid)
            {
                ModelState.AddModelError("Input.Code", ResL.Validation_InvalidCode);
                
                await InitializeModel(user);
                return Page();
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);

            logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);
            StatusMessage = new StatusMessageModel(ResL.Message_AuthentificatorVerificationSuccessful, true).ToJson();

            if (await userManager.CountRecoveryCodesAsync(user) == 0)
            {
                RecoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToArray();

                return RedirectToPage("./ShowRecoveryCodes");
            }

            return RedirectToPage("./TwoFactorAuthentication");
        }

        #endregion

        #region Methods

        private async Task InitializeModel(User user)
        {
            // load the authenticator key & QR code URI to display on the form
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);

            AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private static string FormatKey(string unformattedKey)
        {
            return string.Join(" ", unformattedKey.ToUpperInvariant().Split(4));
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(AuthenticatorUriFormat, urlEncoder.Encode("Web-License"), urlEncoder.Encode(email), unformattedKey);
        }

        #endregion
    }
}
