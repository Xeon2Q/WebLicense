using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XInputFloatingTextBase : InputText
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string CssClass2 { get; set; }
    }
}