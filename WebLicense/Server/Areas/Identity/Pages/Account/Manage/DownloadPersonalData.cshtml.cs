using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Server.Auxiliary.Extensions;
using ResG = WebLicense.Server.Resources.Global;
using ResL = WebLicense.Server.Resources.Areas_Identity_Pages_Account_Manage_DownloadPersonalDataModel;

namespace WebLicense.Server.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        #region Actions

        public async Task<IActionResult> OnPostAsync([FromServices] UserManager<User> userManager, [FromServices] ILogger<DownloadPersonalDataModel> logger)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound(string.Format(ResG.Error_Format_UnableLoadUserWithId, userManager.GetUserId(User)));

            logger.With(LogAction.DownloadPersonalData, user).LogInformation(ResL.Message_UserDownloadingPersonalData, userManager.GetUserId(User));

            var personalData = await GetPersonalData(userManager, user);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            Response.Headers.Add("Content-Disposition", $"attachment; filename={HtmlEncoder.Default.Encode(ResG.ApplicationName)}-PersonalData.json");
            return new FileContentResult(bytes, "application/json");
        }

        #endregion

        #region Methods

        private static async Task<IDictionary<string, string>> GetPersonalData(UserManager<User> userManager, User user)
        {
            var data = new Dictionary<string, string>();

            var properties = typeof(User).GetProperties().Where(q => Attribute.IsDefined(q, typeof(PersonalDataAttribute)));
            foreach (var p in properties)
            {
                data.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                data.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            return data;
        }

        #endregion
    }
}
