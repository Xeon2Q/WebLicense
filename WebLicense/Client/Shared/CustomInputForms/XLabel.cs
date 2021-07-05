using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebLicense.Client.Shared.CustomInputForms
{
    public class XLabel : ComponentBase
    {
        #region Properties

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public FieldIdentifier Field { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> ExtraAttributes { get; set; }

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (string.IsNullOrWhiteSpace(Field.FieldName)) return;

            Text = Field.Model.GetType().GetProperty(Field.FieldName)?.GetCustomAttribute<DisplayAttribute>()?.GetName();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            var x = 0;
            builder.OpenElement(x++, "label");

            if (ExtraAttributes != null && ExtraAttributes.Count > 0)
            {
                foreach (var ea in ExtraAttributes)
                {
                    builder.AddAttribute(x++, ea.Key, ea.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(Text))
            {
                builder.AddContent(x, Text);
            }

            builder.CloseElement();
        }

        #endregion
    }
}