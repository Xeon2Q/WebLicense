using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Server.Auxiliary.IdentityServices
{
    public sealed class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        #region C-tor | Fields

        private readonly IMediator mediator;

        public ClaimsPrincipalFactory(IMediator mediator, UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<IdentityOptions> identityOptions) : base(userManager, roleManager, identityOptions)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        #region IUserClaimsPrincipalFactory overrides

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaims(await GetClaims(user));

            return identity;
        }

        #endregion

        #region Methods

        private async Task<IEnumerable<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>();

            if (user == null) return claims;

            claims.Add(new(JwtClaimTypes.Id, user.Id.ToString()));
            claims.Add(new(JwtClaimTypes.Name, user.UserName));

            // roles
            var roleNames = await UserManager.GetRolesAsync(user) ?? new List<string>(0);
            claims.AddRange(roleNames.Select(q => new Claim(JwtClaimTypes.Role, q)));

            // claims
            var result = await mediator.Send(new Logic.UseCases.Users.GetClaims(user));
            if (result.Succeeded && result.Data.Count > 0) claims.AddRange(result.Data);

            return claims;
        }

        #endregion
    }
}