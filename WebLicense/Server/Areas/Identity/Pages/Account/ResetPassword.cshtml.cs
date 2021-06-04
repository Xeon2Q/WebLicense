using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Shared.Auxiliary.Claims;
using WebLicense.Shared.Identity;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ResetPasswordModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
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
            [Compare("Password", ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordMismatch")]
            [Display(Name = "Model_ConfirmPassword", ResourceType = typeof(ResL))]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion

        #region Actions

        public IActionResult OnGet(string code = default)
        {
            if (code == null) return BadRequest(ResL.Error_MissingCode);

            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<ResetPasswordModel> logger, [FromServices] IMediator mediator, [FromServices] UserManager<User> userManager)
        {
            // return with errors
            if (!ModelState.IsValid) return Page();

            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var canResetPassword = await ResetPasswordAllowed(mediator, user, logger);
            if (!canResetPassword)
            {
                return Forbid();
            }

            var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            
            // reset successful
            if (result.Succeeded) return RedirectToPage("./ResetPasswordConfirmation");

            // show errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        #endregion

        #region Methods

        private async Task<bool> ResetPasswordAllowed(ISender mediator, User user, ILogger logger)
        {
            var result = await mediator.Send(new HasClaim(new UserInfo(user), WLClaims.OwnAccount.CanResetPassword.ClaimType, bool.TrueString));
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogErrorWith(LogAction.Account.Profile.ChangePassword, user, error, error.Message);
                }
            }

            return result.Succeeded && result.Data;
        }

        #endregion
    }
}
