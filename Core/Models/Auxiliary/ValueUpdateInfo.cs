using System;
using System.Text.Json.Serialization;

namespace WebLicense.Core.Models.Auxiliary
{
    public sealed class ValueUpdateInfo
    {
        public int CultureId { get; set; }

        public int Order { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [JsonConstructor]
        public ValueUpdateInfo(int cultureId, int order, string oldValue, string newValue)
        {
            CultureId = cultureId;
            Order = order;
            OldValue = oldValue;
            NewValue = newValue;
        }

        [JsonConstructor]
        public ValueUpdateInfo(int cultureId, int order, object oldValue, object newValue) : this(cultureId, order, oldValue != null ? Convert.ToString(oldValue) : null, newValue != null ? Convert.ToString(newValue) : null)
        {
        }
    }
}