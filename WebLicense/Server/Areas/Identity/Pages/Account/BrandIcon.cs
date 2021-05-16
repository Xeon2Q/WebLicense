using System.Collections.Generic;
using System.Linq;

namespace WebLicense.Server.Areas.Identity.Pages.Account
{
    public static class BrandIcon
    {
        private const string NoBrand = "bi bi-star";

        private static readonly ILookup<string, string> Icons = new Dictionary<string, string>
        {
            {"microsoft", "bi bi-grid"}
        }.ToLookup(k => k.Key, v => v.Value);

        public static string Get(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider)) return NoBrand;

            var icon = Icons[provider.ToLower()].FirstOrDefault();

            return icon ?? NoBrand;
        }
    }
}