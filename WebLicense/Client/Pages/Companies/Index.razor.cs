using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Radzen;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WebLicense.Client.Auxiliary;
using WebLicense.Client.Auxiliary.Extensions;
using WebLicense.Shared;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Pages.Companies
{
    public partial class Index
    {
        #region C-tor | Properties

        [Inject]
        public JsLog Log { get; set; }

        [Inject]
        private HttpClient Client { get; set; }

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
                await Log.LogAsync(JsonSerializer.Serialize(args));

                var parameters = new List<string>();
                if (args.Skip > 0) parameters.Add($"skip={args.Skip.Value}");
                if (args.Top > 0) parameters.Add($"take={args.Top.Value}");
                if (args.Filters?.Any() ?? false) parameters.Add($"filters={args.FiltersToUrlEncodedString()}");
                if (args.Sorts?.Any() ?? false) parameters.Add($"sorts={args.SortsToUrlEncodedString()}");

                var data = await Client.GetFromJsonAsync<ListData<CompanyInfo>>($"api/companies?{string.Join('&', parameters)}");

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