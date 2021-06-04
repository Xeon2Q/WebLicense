using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Shared.Identity;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_RegisterModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [Display(Name = "Model_Name", ResourceType = typeof(ResL))]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Model_Email", ResourceType = typeof(ResL))]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 8, ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordLength")]
            [DataType(DataType.Password)]
            [Display(Name = "Model_Password", ResourceType = typeof(ResL))]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordsMismatch")]
            [Display(Name = "Model_ConfirmPassword", ResourceType = typeof(ResL))]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Model_CustomerReferenceId", ResourceType = typeof(ResL))]
            public string CustomerReferenceId { get; set; }

            [Required]
            [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_GdprNotAccepted")]
            [Display(Name = "Model_GDPR", ResourceType = typeof(ResL))]
            public bool GDPR { get; set; }

            [Required]
            [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_EulaNotAccepted")]
            [Display(Name = "Model_Eula", ResourceType = typeof(ResL))]
            public bool EULA { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly SignInManager<User> _signInManager;

        public RegisterModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        #endregion

        #region Actions

        public async Task OnGetAsync(string returnUrl = default)
        {
            await InitializeModel(returnUrl);
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<RegisterModel> logger, [FromServices] IMediator mediator, [FromServices] UserManager<User> userManager, string returnUrl = default)
        {
            await InitializeModel(returnUrl);

            // return with errors
            if (!ModelState.IsValid) return Page();

            var user = await TryCreateUser(mediator);
            if (user == null) return Page();

            // user created successfully
            logger.LogInformation("User created a new account with password.");

            await SendEmailConfirmation(mediator, userManager, user);

            // confirm account
            if (userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new {email = user.Email});
            }

            // confirmation not needed
            await _signInManager.SignInAsync(user, false);

            return LocalRedirect(returnUrl);
        }

        #endregion

        #region Methods

        private async Task InitializeModel(string returnUrl = default)
        {
            ReturnUrl = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl.Trim() : Url.Content("~/");

            // clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        private async Task<User> TryCreateUser(ISender mediator)
        {
            var result = await mediator.Send(new AddUser(GetUserInfo(Input), Input.Password, Input.CustomerReferenceId));

            // success
            if (result.Succeeded) return result.Data;

            // failure
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return null;
        }

        private static UserInfo GetUserInfo(InputModel input)
        {
            return new()
            {
                Email = input.Email,
                UserName = input.UserName,
                GdprAccepted = input.GDPR,
                EulaAccepted = input.EULA
            };
        }

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
