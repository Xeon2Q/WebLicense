using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace WebLicense.Client.Auxiliary.Configuration
{
    public sealed class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        #region C-tor

        public CustomAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        #endregion

        #region AccountClaimsPrincipalFactory overrides

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (user == null || user.Identity?.IsAuthenticated != true) return user;
            if (user.Identity is not ClaimsIdentity identity) return user;

            AddRoleClaims(identity, account, options);
            AddClaims(identity, account);

            return user;
        }

        #endregion

        #region Methods

        private static void AddRoleClaims(ClaimsIdentity identity, RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var existingRoles = identity.FindAll(identity.RoleClaimType).ToArray();
            if (existingRoles.Length == 0) return;

            // remove existing role-claims
            foreach (var role in existingRoles) identity.RemoveClaim(role);

            var rolesElement = account.AdditionalProperties[identity.RoleClaimType];
            if (rolesElement is not JsonElement roles) return;

            if (roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray().Select(q => q.GetString()).Where(q => q != null))
                {
                    identity.AddClaim(new(options.RoleClaim, role));
                }
            }
            else
            {
                var role = roles.GetString();
                if (role != null) identity.AddClaim(new(options.RoleClaim, role));
            }
        }

        private static void AddClaims(ClaimsIdentity identity, RemoteUserAccount account)
        {
            var claims = identity.FindAll(q => q.Type != identity.NameClaimType && q.Type != identity.RoleClaimType).ToArray();
            if (claims.Length == 0) return;

            // remove existing user-claims
            foreach (var claim in claims) identity.RemoveClaim(claim);

            var items = account.AdditionalProperties.Where(q => q.Key != identity.NameClaimType && q.Key != identity.RoleClaimType && q.Value != null).ToList();
            foreach (var item in items)
            {
                identity.AddClaim(new(item.Key, item.Value?.ToString() ?? string.Empty));
            }
        }

        #endregion
    }
}