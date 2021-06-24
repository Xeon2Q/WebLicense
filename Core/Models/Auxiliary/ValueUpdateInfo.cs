using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WebLicense.Core.Models.Auxiliary
{
    public sealed class ValueUpdateInfo
    {
        [Description("Order of change in list of changes")]
        public int Order { get; set; }

        [Description("Value before change")]
        public string OldValue { get; set; }

        [Description("Value after change")]
        public string NewValue { get; set; }

        [JsonConstructor]
        public ValueUpdateInfo(int order, string oldValue, string newValue)
        {
            Order = order;
            OldValue = oldValue;
            NewValue = newValue;
        }

        [JsonConstructor]
        public ValueUpdateInfo(int order, object oldValue, object newValue) : this(order, oldValue?.ToString(), newValue?.ToString())
        {
        }
    }
}