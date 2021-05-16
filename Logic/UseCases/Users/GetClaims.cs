using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetClaims : IRequest<CaseResult<IList<Claim>>>
    {
        internal readonly Role Role;
        internal readonly User User;

        public GetClaims(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }

        public GetClaims(Role role)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
        }
    }

    internal sealed class GetClaimsHandler : IRequestHandler<GetClaims, CaseResult<IList<Claim>>>
    {
        private readonly DatabaseContext db;

        public GetClaimsHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<IList<Claim>>> Handle(GetClaims request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Role != null)
                {
                    var claims = await GetRoleClaims(request.Role, cancellationToken);
                    return new CaseResult<IList<Claim>>(claims);
                }

                if (request.User != null)
                {
                    var claims = await GetUserClaims(request.User, cancellationToken);
                    return new CaseResult<IList<Claim>>(claims);
                }

                throw new Exception("We should not be here");
            }
            catch (Exception e)
            {
                return new CaseResult<IList<Claim>>(e);
            }
        }

        #region Methods

        private async Task<IList<Claim>> GetRoleClaims(Role role, CancellationToken cancellationToken)
        {
            return await db.RoleClaims.Where(q => q.RoleId == role.Id).Select(q => new Claim(q.ClaimType, q.ClaimValue)).ToListAsync(cancellationToken);
        }

        private async Task<IList<Claim>> GetUserClaims(User user, CancellationToken cancellationToken)
        {
            // claims given by roles
            var roleClaims = await db.UserRoles.Where(q => q.UserId == user.Id)
                                     .Join(db.RoleClaims, r => r.RoleId, c => c.RoleId, (r, c) => c)
                                     .Select(q => new Claim(q.ClaimType, q.ClaimValue)).ToListAsync(cancellationToken);

            // claims given by user
            var userClaims = await db.UserClaims.Where(q => q.UserId == user.Id).Select(q => new Claim(q.ClaimType, q.ClaimValue)).ToListAsync(cancellationToken);
            if (userClaims.Count == 0) return roleClaims;

            // override roleClaims with values from userClaims
            var userClaimTypes = userClaims.Select(q => q.Type).Distinct().ToList();
            roleClaims = roleClaims.Where(q => !userClaimTypes.Contains(q.Type)).ToList();
            roleClaims.AddRange(userClaims);

            return roleClaims;
        }

        #endregion
    }
}