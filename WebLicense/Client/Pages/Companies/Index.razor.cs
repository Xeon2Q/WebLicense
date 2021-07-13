using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Radzen;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Pages.Companies
{
    public partial class Index
    {
        #region C-tor | Properties

        [Inject]
        private HttpClient Client { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<CompanyInfo> Data { get; set; }

        public bool IsLoading { get; set; }

        #endregion

        #region Methods

        public async Task LoadDataAsync(LoadDataArgs args)
        {
            IsLoading = true;

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"skip", args.Skip},
                    {"top", args.Top},
                    {"filters", args.FiltersToUrlEncodedString()},
                    {"sorts", args.SortsToUrlEncodedString()}
                }.Select(q => (q.Key, q.Value)).ToArray();

                var data = await Client.GetJson<ListData<CompanyInfo>>($"{Navigation.BaseUri}api/companies", parameters);

                TotalCount = data?.Total ?? 0;
                Data = data?.Data;
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }

            IsLoading = false;
        }

        #endregion
    }
}