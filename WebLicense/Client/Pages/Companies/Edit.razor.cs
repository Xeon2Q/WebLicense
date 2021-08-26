using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebLicense.Client.Auxiliary;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Pages.Companies
{
    public partial class Edit : ComponentBase
    {
        #region C-tor | Properties

        [Parameter]
        public int Id { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private HttpClient Client { get; set; }

        public CompanyInfo Data { get; set; } = new();

        [Inject]
        public JsUtils Js { get; set; }

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            Data = await GetCompanyInfo(Id);
        }

        #endregion

        #region Private methods

        private async Task<CompanyInfo> GetCompanyInfo(int id)
        {
            try
            {
                return await Client.GetJson<CompanyInfo>($"{Navigation.BaseUri}api/companies/{id}");
            }
            catch (Exception e)
            {
                await Js.LogAsync(e.Message);
                return null;
            }
        }

        protected async Task Save(CompanyInfo info)
        {
            try
            {
                await Client.Patch($"{Navigation.BaseUri}api/companies", info);

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