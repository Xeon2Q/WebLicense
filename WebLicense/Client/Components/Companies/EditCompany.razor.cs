using System.Linq;
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

        public void DeleteUser(CompanyUserInfo item)
        {
            if (Data?.Users == null || item == null) return;

            Data.Users = Data.Users.Where(q => q != item).ToList();
        }

        public void ChangeUser(long? id, bool? manager)
        {
            if (Data?.Users == null || id == null) return;

            var user = Data.Users.FirstOrDefault(q => q.Id == id.Value);
            if (user == null) return;

            if (manager.HasValue) user.IsManager = manager.Value;
        }

        public void AddInvitedUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            var users = Data.Users.ToList();
            users.Add(new CompanyUserInfo{Email = email.Trim(), Id = 0, IsInvite = true, IsManager = false, Name = string.Empty});

            Data.Users = users;

            StateHasChanged();
        }

        #endregion
    }
}