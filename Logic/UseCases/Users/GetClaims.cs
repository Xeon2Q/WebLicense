using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetClaims : IRequest<CaseResult<IList<Claim>>>, IValidate
    {
        internal readonly Role Role;
        internal readonly UserInfo User;

        public GetClaims(UserInfo user)
        {
            User = user;
        }

        public GetClaims(Role role)
        {
            Role = role;
        }

        public void Validate()
        {
            if (User == null && Role == null) throw new CaseException(Exceptions.Role_and_User_Null, "Both 'User' and 'Role' are null");
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
                request.Validate();

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

                throw new CaseException("We should not be here");
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

        private async Task<IList<Claim>> GetUserClaims(UserInfo user, CancellationToken cancellationToken)
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