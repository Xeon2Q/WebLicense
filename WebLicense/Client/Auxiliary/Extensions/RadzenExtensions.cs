using System.Linq;
using System.Net;
using System.Text.Json;

namespace WebLicense.Client.Auxiliary.Extensions
{
    public static class RadzenExtensions
    {
        #region Common methods

        private static string EncodeJsonAndUrl(object obj)
        {
            return obj != null ? WebUtility.UrlEncode(JsonSerializer.Serialize(obj)) : null;
        }

        #endregion

        #region LoadDataArgs

        public static string FiltersToUrlEncodedString(this Radzen.LoadDataArgs args)
        {
            return args?.Filters != null && args.Filters.Any() ? EncodeJsonAndUrl(args.Filters) : string.Empty;
        }

        public static string SortsToUrlEncodedString(this Radzen.LoadDataArgs args)
        {
            return args?.Sorts != null && args.Sorts.Any() ? EncodeJsonAndUrl(args.Sorts) : string.Empty;
        }

        #endregion
    }
}