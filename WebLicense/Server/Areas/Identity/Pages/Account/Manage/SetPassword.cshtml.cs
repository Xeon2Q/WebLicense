using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_SetPasswordModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_NewPassword", ResourceType = typeof(ResL))]
            [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordLength")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Model_ConfirmPassword", ResourceType = typeof(ResL))]
            [Compare(nameof(NewPassword), ErrorMessageResourceType = typeof(ResL), ErrorMessageResourceName = "Validation_PasswordsMismatch")]
            public string ConfirmPassword { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public SetPasswordModel(UserManager<User> userManager)
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
            if (hasPassword) return RedirectToPage("./ChangePassword");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<SetPasswordModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            // return with errors
            if (!ModelState.IsValid) return Page();

            var result = await userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.With(LogAction.SetOwnPassword, user).LogError(error.Description);

                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await signInManager.RefreshSignInAsync(user);

            logger.With(LogAction.SetOwnPassword, user).LogInformation(string.Format(ResL.Log_Success, user.Id));

            StatusMessage = "Your password has been set.";

            return RedirectToPage();
        }

        #endregion
    }
}
