using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MediatR;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Server.Auxiliary.PageModels;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_EmailModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Text_NewEmail", ResourceType = typeof(ResL))]
            public string NewEmail { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public EmailModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region Properties

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string Username { get; set; }

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

            InitializeModel(user);

            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync([FromServices] IMediator mediator)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // return with errors
            if (!ModelState.IsValid)
            {
                InitializeModel(user);

                return Page();
            }

            // change user email
            var email = await userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                await ChangeUserEmail(mediator, user, Input.NewEmail);

                StatusMessage = new StatusMessageModel(ResL.Message_ConfirmationEmailSent, true).ToJson();
            }
            else
            {
                StatusMessage = new StatusMessageModel(ResL.Message_EmailNotChanged, true).ToJson();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync([FromServices] IMediator mediator)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // return with errors
            if (!ModelState.IsValid)
            {
                InitializeModel(user);

                return Page();
            }

            // send verification email
            await VerifyUserEmail(mediator, user);

            StatusMessage = new StatusMessageModel(ResL.Message_VerificationEmailSent, true).ToJson();

            return RedirectToPage();
        }

        #endregion

        #region Methods

        private void InitializeModel(User user)
        {
            Email = user.Email;

            Input = new InputModel {NewEmail = null};

            IsEmailConfirmed = user.EmailConfirmed;
        }

        private async Task ChangeUserEmail(ISender mediator, User user, string newEmail)
        {
            var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmailChange", null, new {area = "Identity", userId = user.Id, email = Input.NewEmail, code}, Request.Scheme);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            await SendEmailConfirmation(mediator, user, newEmail, callbackUrl);
        }

        private async Task VerifyUserEmail(ISender mediator, User user)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new {area = "Identity", userId = user.Id, code}, Request.Scheme);
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);

            await SendEmailConfirmation(mediator, user, null, callbackUrl);
        }

        private async Task SendEmailConfirmation(ISender mediator, User user, string newEmail, string callbackUrl)
        {
            await mediator.Send(new SendEmailUserEmailConfirmation(user, newEmail, callbackUrl));
        }

        #endregion
    }
}
