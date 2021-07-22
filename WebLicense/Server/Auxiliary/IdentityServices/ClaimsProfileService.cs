using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace WebLicense.Server.Auxiliary.IdentityServices
{
    public sealed class ClaimsProfileService : IProfileService
    {
        #region IProfileService implementation

        public ClaimsProfileService()
        {
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var ids = context.Subject.FindAll(JwtClaimTypes.Id);
            context.IssuedClaims.AddRange(ids);

            var names = context.Subject.FindAll(JwtClaimTypes.Name);
            context.IssuedClaims.AddRange(names);

            var roles = context.Subject.FindAll(JwtClaimTypes.Role);
            context.IssuedClaims.AddRange(roles);

            await Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}