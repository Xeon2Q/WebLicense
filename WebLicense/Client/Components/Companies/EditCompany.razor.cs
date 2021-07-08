using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Components.Companies
{
    public partial class EditCompany : ComponentBase
    {
        #region Properties

        [Parameter]
        public CompanyInfo Data { get; set; }

        [Parameter]
        public EventCallback<CompanyInfo> DataChanged { get; set; }

        #endregion

        #region Methods

        private async Task SyncChanges()
        {
            if (DataChanged.HasDelegate) await DataChanged.InvokeAsync(Data);
        }

        #endregion
    }
}