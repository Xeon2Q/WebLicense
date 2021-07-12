using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLicense.Shared.Companies;

namespace WebLicense.Client.Components.Companies
{
    public partial class EditCompany : ComponentBase
    {
        #region Properties

        [Parameter]
        public bool IsAdd { get; set; } = false;

        [Parameter]
        public CompanyInfo Data { get; set; }

        [Parameter]
        public EventCallback<CompanyInfo> DataChanged { get; set; }

        [Parameter]
        public string SubmitButtonClass { get; set; }

        [Parameter]
        public string SubmitButtonText { get; set; }

        #endregion

        #region Methods

        private async Task SyncChanges()
        {
            if (DataChanged.HasDelegate) await DataChanged.InvokeAsync(Data);
        }

        private void SaveCallback(EditContext obj)
        {

        }

        #endregion
    }
}