using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XInputNumberBase<T> : InputBase<T>
    {
        [Parameter]
        public string CssClass2 { get; set; }

        [Parameter]
        public string Label { get; set; }

        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            validationErrorMessage = null;

            var tt = typeof(T);
            dynamic d = default(T);

            if (!string.IsNullOrWhiteSpace(value))
            {
                if ((tt == typeof(int?) || tt == typeof(int)) && int.TryParse(value, out var a1))
                {
                    d = a1;
                }
                else if ((tt == typeof(long?) || tt == typeof(long)) && long.TryParse(value, out var a2))
                {
                    d = a2;
                }
                else if ((tt == typeof(decimal?) || tt == typeof(decimal)) && decimal.TryParse(value, out var a3))
                {
                    d = a3;
                }
                else if ((tt == typeof(float?) || tt == typeof(float)) && float.TryParse(value, out var a4))
                {
                    d = a4;
                }
                else if ((tt == typeof(double?) || tt == typeof(double)) && double.TryParse(value, out var a5))
                {
                    d = a5;
                }
            }

            result = (T) d;

            return true;
        }
    }
}