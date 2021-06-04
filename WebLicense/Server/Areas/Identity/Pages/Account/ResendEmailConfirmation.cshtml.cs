using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Shared.Identity;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_ResendEmailConfirmationModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
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

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<ResendEmailConfirmationModel> logger, [FromServices] IMediator mediator, [FromServices] UserManager<User> userManager)
        {
            // return with errors
            if (!ModelState.IsValid) return Page();

            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, ResL.Message_CheckEmailForVerificationCode);
                return Page();
            }

            await SendEmailMessage(mediator, userManager, user);

            ModelState.AddModelError(string.Empty, ResL.Message_CheckEmailForVerificationCode);
            return Page();
        }

        #endregion

        #region Methods

        private async Task SendEmailMessage(ISender mediator, UserManager<User> userManager, User user)
        {
            var userId = await userManager.GetUserIdAsync(user);

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new {userId, code}, Request.Scheme);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            await mediator.Send(new SendEmailUserEmailConfirmation(new UserInfo(user), null, callbackUrl));
        }

        #endregion
    }
}
