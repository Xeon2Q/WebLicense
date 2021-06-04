using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Shared.Identity;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ExternalLoginModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        #endregion

        #region Actions

        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost([FromServices] SignInManager<User> signInManager, string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", "Callback", new {returnUrl});
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<ExternalLoginModel> logger, string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = string.Format(ResL.Error1, remoteError);
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = ResL.Error2;
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            // sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded)
            {
                var userName = info.Principal?.Identity?.Name ?? string.Empty;
                logger.LogInformationWith(LogAction.Account.Login.External, new User {UserName = userName}, ResL.Log1, userName, info.LoginProvider);

                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            // if the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync([FromServices] IMediator mediator, [FromServices] SignInManager<User> signInManager, [FromServices] UserManager<User> userManager, [FromServices] ILogger<ExternalLoginModel> logger, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // get the information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = ResL.Error3;
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            if (ModelState.IsValid)
            {
                var user = new User {UserName = Input.Email, Email = Input.Email};

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        logger.LogInformationWith(LogAction.Account.Registration.External, user, ResL.Log2, info.LoginProvider);

                        await SendEmailConfirmation(mediator, userManager, user);

                        // if account confirmation is required, we need to show the link if we don't have a real email sender
                        if (userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new {Input.Email});
                        }

                        await signInManager.SignInAsync(user, false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }

                // something went wrong
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;

            return Page();
        }

        #endregion

        #region Methods

        private async Task SendEmailConfirmation(ISender mediator, UserManager<User> userManager, User user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new {area = "Identity", userId = user.Id, code, ReturnUrl}, Request.Scheme);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            await mediator.Send(new SendEmailUserEmailConfirmation(new UserInfo(user), null, callbackUrl));
        }

        #endregion
    }
}
