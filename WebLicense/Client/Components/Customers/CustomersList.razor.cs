using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebLicense.Shared.Customers;

namespace WebLicense.Client.Components.Customers
{
    public partial class CustomersList
    {
        #region Properties

        [Parameter]
        public int TotalCount { get; set; }

        [Parameter]
        public CustomerInfo[] Data { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            if (TotalCount < 0) TotalCount = 0;
            Data ??= new CustomerInfo[0];
        }

        public async Task LoadDataAsync(LoadDataArgs args)
        {
        }

        #endregion
    }
}