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
        public EventCallback<CompanyInfo> OnSave { get; set; }

        [Parameter]
        public string SubmitButtonClass { get; set; }

        [Parameter]
        public string SubmitButtonText { get; set; }

        #endregion

        #region Methods

        public void SaveCallback(EditContext ecx)
        {
            OnSave.InvokeAsync(Data);
        }

        public void InvalidCallback(EditContext ecx)
        {
        }

        #endregion
    }
}