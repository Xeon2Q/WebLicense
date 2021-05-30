using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_DeletePersonalDataModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Model_Password", ResourceType = typeof(ResL))]
            public string Password { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly UserManager<User> userManager;

        public DeletePersonalDataModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RequirePassword { get; set; }

        #endregion

        #region Actions

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            RequirePassword = await userManager.HasPasswordAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<User> signInManager, [FromServices] ILogger<DeletePersonalDataModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            RequirePassword = await userManager.HasPasswordAsync(user);
            if (RequirePassword && !await userManager.CheckPasswordAsync(user, Input.Password))
            {
                ModelState.AddModelError(string.Empty, ResL.Validation_InvalidPassword);
                return Page();
            }

            var userId = user.Id;
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                logger.LogErrorWith(LogAction.Account.Delete, user, null, ResL.Error_UnexpectedError, userId);

                throw new InvalidOperationException(string.Format(ResL.Error_UnexpectedError, userId));
            }

            logger.LogInformationWith(LogAction.Account.Delete, user, null, ResL.Message_UserDeleted, userId);

            await signInManager.SignOutAsync();

            return Redirect("~/");
        }

        #endregion
    }
}
