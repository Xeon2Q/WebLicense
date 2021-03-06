using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XInputFloatingTextBase : InputText, IXInputBase
    {
        public string UUID { get; } = $"XID-{Guid.NewGuid():N}";

        [Parameter]
        public string CssClass2 { get; set; }
    }
}