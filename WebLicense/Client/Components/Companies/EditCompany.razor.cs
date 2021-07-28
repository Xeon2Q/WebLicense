using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLicense.Client.Auxiliary;
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
        public CompanyAccessInfo Access { get; set; }

        [Parameter]
        public EventCallback<CompanyInfo> DataChanged { get; set; }

        [Parameter]
        public EventCallback<CompanyInfo> OnSave { get; set; }

        [Parameter]
        public string SubmitButtonClass { get; set; }

        [Parameter]
        public string SubmitButtonText { get; set; }

        [Inject]
        public JsUtils Js { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
        }

        public void SaveCallback(EditContext ecx)
        {
            OnSave.InvokeAsync(Data);
        }

        public void InvalidCallback(EditContext ecx)
        {
        }

        public void DeleteUser(CompanyUserInfo item)
        {
            if (Data?.Users == null || item == null) return;

            Data.Users = Data.Users.Where(q => q != item).ToList();
        }

        public void ChangeUser(string email, bool? manager)
        {
            if (Data?.Users == null || !manager.HasValue || string.IsNullOrWhiteSpace(email)) return;

            var user = Data.Users.FirstOrDefault(q => string.Equals(q.Email, email, StringComparison.OrdinalIgnoreCase));
            
            if (user != null) user.IsManager = manager.Value;
        }

        public void AddInvitedUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            var users = Data.Users.ToList();
            if (users.Any(q => string.Equals(q.Email, email, StringComparison.OrdinalIgnoreCase))) return;

            users.Add(new CompanyUserInfo{Email = email.Trim(), Id = 0, IsInvite = true, IsManager = false, Name = string.Empty});

            Data.Users = users;

            StateHasChanged();
        }

        #endregion
    }
}