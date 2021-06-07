using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using WebLicense.Shared.Auxiliary.Policies;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_ChangePasswordModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = WLPolicies.Account.Names.ChangePassword)]
    public class ChangePasswordModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_CurrentPassword", ResourceType = typeof(ResL))]
            public string OldPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_NewPassword", ResourceType = typeof(ResL))]
            [StringLength(100, MinimumLength = 8, ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordLength")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_ConfirmNewPassword", ResourceType = typeof(ResL))]
            [Compare("NewPassword", ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordMismatch")]
            public string ConfirmPassword { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public ChangePasswordModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            var hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword) return RedirectToPage("./SetPassword");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<ChangePasswordModel> logger)
        {
            // return with errors
            if (!ModelState.IsValid) return Page();

            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // attempt to change password
            var result = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            // attempt > failed
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogErrorWith(LogAction.Account.Profile.ChangePassword, user, null, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            // attempt > successful
            await signInManager.RefreshSignInAsync(user);

            logger.LogInformationWith(LogAction.Account.Profile.ChangePassword, user, ResL.Message_PasswordChangeSuccessful);

            StatusMessage = new StatusMessageModel(ResL.Message_PasswordChanged, true).ToJson();

            return RedirectToPage();
        }

        #endregion
    }
}
