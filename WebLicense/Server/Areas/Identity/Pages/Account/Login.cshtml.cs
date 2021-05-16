using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_LoginModel;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Model_Email", ResourceType = typeof(ResL))]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_Password", ResourceType = typeof(ResL))]
            public string Password { get; set; }

            [Display(Name = "Model_RememberMe", ResourceType = typeof(ResL))]
            public bool RememberMe { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        #endregion

        #region Actions

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage.Trim());
            }

            await InitializeModel(returnUrl);
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<LoginModel> logger, string returnUrl = null)
        {
            await InitializeModel(returnUrl);

            // return with errors
            if (!ModelState.IsValid) return Page();

            var (result, user) = await TrySignInUser();
            if (result.Succeeded)
            {
                logger.With(LogAction.LoginAttempt, user).LogInformation("User logged in");

                return LocalRedirect(ReturnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl, Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                logger.With(LogAction.LoginAttempt, user).LogWarning("User account is locked out");

                return RedirectToPage("./Lockout");
            }

            // some errors happened
            ModelState.AddModelError(string.Empty, ResL.Error_InvalidLoginAttempt);
            return Page();
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

        private async Task<(SignInResult, User)> TrySignInUser()
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);

            return (await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, true), user);
        }

        #endregion
    }
}
