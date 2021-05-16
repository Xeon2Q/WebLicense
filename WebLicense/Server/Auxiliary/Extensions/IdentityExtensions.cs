using System;
using System.Security.Claims;
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
                return Convert.ChangeType(principal.GetSubjectId(), typeof(T)) is T id ? id : default;
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