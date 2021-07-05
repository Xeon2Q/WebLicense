using System;
using Microsoft.AspNetCore.Components.Forms;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XInputTextBase : InputText, IXInputBase
    {
        public string UUID { get; } = $"XID-{Guid.NewGuid():N}";

        public string CssClass2 { get; set; }
    }
}