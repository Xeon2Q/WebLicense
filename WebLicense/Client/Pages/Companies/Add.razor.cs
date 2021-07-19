using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using WebLicense.Client.Auxiliary;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Pages.Companies
{
    public partial class Add : ComponentBase
    {
        #region C-tor | Properties

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private HttpClient Client { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public CompanyInfo Data { get; set; } = new();

        [Inject]
        public JsUtils Js { get; set; }

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
                Settings = null,
                Users = new List<CompanyUserInfo>
                {
                    new() {Id = user.GetId(), Name = user.GetName(), Email = user.GetEmail(), IsManager = true}
                }
            };

            return entity;
        }

        protected async Task Save(CompanyInfo info)
        {
            try
            {
                await Client.Post($"{Navigation.BaseUri}api/companies", info);

                Navigation.NavigateTo("/companies");
            }
            catch (Exception e)
            {
                await Js.LogAsync(e.Message);
            }
        }

        #endregion
    }
}