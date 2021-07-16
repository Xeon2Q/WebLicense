using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLicense.Shared.Resources;

namespace WebLicense.Client.Components.Companies
{
    public partial class InviteUser : ComponentBase
    {
        #region Properties

        public EditContext Context { get; set; }

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [Parameter]
        [Required]
        [EmailAddress]
        [Display(Name = "User_Email", ResourceType = typeof(Model))]
        public string Email { get; set; }

        [Parameter]
        public string CompanyName { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            Context = new EditContext(Email);
        }

        public void Submit()
        {
            OnSubmit.InvokeAsync(Email);
        }

        public void SaveCallback()
        {
        }

        public void InvalidCallback()
        {
        }

        #endregion
    }
}