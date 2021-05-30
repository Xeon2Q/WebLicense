using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using WebLicense.Server.Auxiliary.PageModels;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_Index;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        #region InputModel

        public class InputModel
        {
            [Required]
            [Display(Name = "Model_Name", ResourceType = typeof(ResL))]
            public string UserName { get; set; }

            [Phone]
            [Display(Name = "Model_PhoneNumber", ResourceType = typeof(ResL))]
            public string PhoneNumber { get; set; }
        }

        #endregion

        #region C-tor | Fields

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        #endregion

        #region Properties

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        #endregion

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, _userManager.GetUserId(User)));

            InitializeModel(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] ILogger<IndexModel> logger)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, _userManager.GetUserId(User)));

            // return with errors
            if (!ModelState.IsValid)
            {
                InitializeModel(user);

                return Page();
            }

            // update the profile and handle the errors
            var result = await UpdateUser(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);

                StatusMessage = new StatusMessageModel(ResL.Message_UpdateProfileSuccessful, true).ToJson();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogErrorWith(LogAction.Account.Profile.Update, user, null, error.Description);
                }

                StatusMessage = new StatusMessageModel(ResL.Error_UpdateProfileFailed, false).ToJson();
            }

            return RedirectToPage();
        }

        #region Methods

        private void InitializeModel(User user)
        {
            Input = new InputModel
            {
                UserName = user?.UserName,
                PhoneNumber = user?.PhoneNumber
            };
        }

        private async Task<IdentityResult> UpdateUser(User user)
        {
            var result1 = await UpdateUserName(user);
            if (!result1.Succeeded) return result1;

            var result2 = await UpdateUserPhone(user);
            return result2;
        }

        private async Task<IdentityResult> UpdateUserName(User user)
        {
            var newName = !string.IsNullOrWhiteSpace(Input.UserName) ? Input.UserName.Trim() : null;
            var oldName = !string.IsNullOrWhiteSpace(user.UserName) ? user.UserName.Trim() : null;

            if (string.Equals(newName, oldName, StringComparison.CurrentCulture)) return IdentityResult.Success;

            return await _userManager.SetUserNameAsync(user, newName);
        }

        private async Task<IdentityResult> UpdateUserPhone(User user)
        {
            var newPhone = !string.IsNullOrWhiteSpace(Input.PhoneNumber) ? Input.PhoneNumber.Trim() : null;
            var oldPhone = !string.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber.Trim() : null;

            if (string.Equals(newPhone, oldPhone, StringComparison.OrdinalIgnoreCase)) return IdentityResult.Success;

            return await _userManager.SetPhoneNumberAsync(user, newPhone);
        }

        #endregion
    }
}
