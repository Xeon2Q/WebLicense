using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLicense.Shared.Customers;

namespace WebLicense.Client.Components.Customers
{
    public partial class EditCustomer : ComponentBase
    {
        #region Properties

        [Parameter]
        public CustomerInfo Data { get; set; }

        [Parameter]
        public EventCallback<CustomerInfo> DataChanged { get; set; }

        #endregion

        #region Methods

        private async Task SyncChanges()
        {
            if (DataChanged.HasDelegate) await DataChanged.InvokeAsync(Data);
        }

        #endregion
    }
}