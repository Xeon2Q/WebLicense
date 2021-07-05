using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XInputCheckBoxBase<T> : InputBase<T>, IXInputBase
    {
        public string UUID { get; } = $"XID-{Guid.NewGuid():N}";

        [Parameter]
        public string CssClass2 { get; set; }

        [Parameter]
        public bool IsLarge { get; set; } = false;

        protected string LabelCssClass { get; set; }

        protected override void OnParametersSet()
        {
            LabelCssClass = "form-check-label ms-" + (IsLarge ? "3" : "2");
        }

        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            validationErrorMessage = null;

            if (bool.TryParse(value, out var b))
            {
                result = (T) Convert.ChangeType(b, typeof(T));
            }
            else
            {
                result = default;
            }

            return true;
        }
    }
}