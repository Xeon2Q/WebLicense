using System;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Extensions;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class IdentityExtensions
    {
        public static T GetId<T>(this ClaimsPrincipal principal)
        {
            if (principal?.Identity == null) return default;

            try
            {
                var sid = principal.FindFirst(q => q.Type == JwtClaimTypes.Id)?.Value;
                if (sid == null) return default;

                return Convert.ChangeType(sid, typeof(T)) is T id ? id : default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static string GetName(this ClaimsPrincipal principal)
        {
            if (principal?.Identity == null) return default;

            try
            {
                return principal.GetDisplayName();
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}