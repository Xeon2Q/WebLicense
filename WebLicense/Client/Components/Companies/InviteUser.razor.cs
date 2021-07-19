using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebLicense.Client.Auxiliary;
using WebLicense.Shared.Resources;

namespace WebLicense.Client.Components.Companies
{
    public partial class InviteUser : ComponentBase
    {
        #region Model

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "User_Email", ResourceType = typeof(Model))]
            public string Email { get; set; }
        }

        #endregion

        #region Properties

        public EditContext Context { get; set; }

        [Inject]
        public JsUtils Js { get; set; }

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [Parameter]
        public InputModel Model { get; set; } = new ();

        [Parameter]
        public string CompanyName { get; set; }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            Context = new EditContext(Model);
        }

        public void ValidSubmit()
        {
            OnSubmit.InvokeAsync(Model?.Email);

            Js.ToggleModalWindow(Id, false);
        }

        public void InvalidSubmit()
        {
        }

        #endregion
    }
}