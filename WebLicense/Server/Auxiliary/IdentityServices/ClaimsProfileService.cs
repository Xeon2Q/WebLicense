using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebLicense.Server.Auxiliary.IdentityServices
{
    public sealed class ClaimsProfileService : IProfileService
    {
        #region IProfileService implementation

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims = new List<Claim>(0);

            var id = context.Subject.GetSubjectId();
            if (string.IsNullOrWhiteSpace(id)) return Task.CompletedTask;

            var names = context.Subject.FindAll(JwtClaimTypes.Name);
            var roles = context.Subject.FindAll(JwtClaimTypes.Role);

            context.IssuedClaims.Add(new(JwtClaimTypes.Id, id));
            context.IssuedClaims.AddRange(names);
            context.IssuedClaims.AddRange(roles);

            //var claims = context.Subject.FindAll(q => q.Type != JwtClaimTypes.Id && q.Type != JwtClaimTypes.Name && q.Type != JwtClaimTypes.Role);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}