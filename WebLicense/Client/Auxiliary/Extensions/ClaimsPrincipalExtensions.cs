using System;
using System.Security.Claims;

namespace WebLicense.Client.Auxiliary.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long? GetId(this ClaimsPrincipal principal)
        {
            const string claimType = "id";

            var claim = principal?.FindFirst(q => claimType.Equals(q.Type, StringComparison.OrdinalIgnoreCase));
            if (claim == null) return null;

            return long.TryParse(claim.Value, out var id) && id > 0 ? id : null;
        }

        public static string GetName(this ClaimsPrincipal principal)
        {
            return principal?.Identity?.Name;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            var claim = principal?.FindFirst(q => ClaimTypes.Email == q.Type);

            return !string.IsNullOrWhiteSpace(claim?.Value) ? claim.Value.Trim() : null;
        }
    }
}