using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Shared.Identity;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ForgotPasswordModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Model_Email", ResourceType = typeof(ResL))]
            public string Email { get; set; }
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<ForgotPasswordModel> logger, [FromServices] IMediator mediator, [FromServices] UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(Input.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    // don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                await SendEmailMessage(mediator, userManager, user);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        #endregion

        #region Methods

        private async Task SendEmailMessage(ISender mediator, UserManager<User> userManager, User user)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ResetPassword", null, new {area = "Identity", code}, Request.Scheme);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            await mediator.Send(new SendEmailUserPasswordResetConfirmation(new UserInfo(user), callbackUrl));
        }

        #endregion
    }
}
