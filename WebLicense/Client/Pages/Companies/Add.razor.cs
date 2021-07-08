using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Pages.Companies
{
    public partial class Add : ComponentBase
    {
        #region C-tor | Properties

        [Inject]
        private HttpClient Client { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public CompanyInfo Data { get; set; } = new();

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            Data = GetEmptyCompanyInfo(state.User);
        }

        #endregion

        #region Private methods

        private static CompanyInfo GetEmptyCompanyInfo(ClaimsPrincipal user)
        {
            var entity = new CompanyInfo
            {
                Id = 0,
                Settings = new CompanySettingsInfo
                {
                    MaxActiveLicensesCount = 1,
                    MaxTotalLicensesCount = 1,
                    ReceiveNotifications = true,
                    NotificationsEmail = user.GetEmail(),
                    CreateActiveLicenses = false,
                    CanActivateLicenses = false,
                    CanDeactivateLicenses = true,
                    CanDeleteLicenses = false,
                    CanActivateMachines = true,
                    CanDeactivateMachines = true,
                    CanDeleteMachines = true
                },
                Users = new List<CompanyUserInfo>
                {
                    new() {Id = user.GetId(), Name = user.GetName()}
                }
            };

            return entity;
        }

        #endregion
    }
}