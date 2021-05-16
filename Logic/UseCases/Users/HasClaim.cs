using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class HasClaim : IRequest<CaseResult<bool>>
    {
        internal readonly User User;
        internal readonly string ClaimType;
        internal readonly string AcceptedValue;

        public HasClaim(User user, string claimType, string acceptedValue)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            ClaimType = claimType;
            AcceptedValue = acceptedValue;
        }
    }

    internal sealed class HasClaimHandler : IRequestHandler<HasClaim, CaseResult<bool>>
    {
        private readonly DatabaseContext db;

        public HasClaimHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<bool>> Handle(HasClaim request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.User.Id < 1 || string.IsNullOrWhiteSpace(request.ClaimType)) return new CaseResult<bool>(false, (string) null);

                // check user claim
                var userClaim = await db.UserClaims.FirstOrDefaultAsync(q => q.UserId == request.User.Id && q.ClaimType == request.ClaimType, cancellationToken);
                if (userClaim != null) return new CaseResult<bool>(string.Equals(userClaim.ClaimValue, request.AcceptedValue, StringComparison.OrdinalIgnoreCase), (string) null);

                // role claims
                var roleClaim = await db.UserRoles.Where(q => q.UserId == request.User.Id)
                                        .Join(db.RoleClaims, r => r.RoleId, c => c.RoleId, (r, c) => c)
                                        .FirstOrDefaultAsync(q => q.ClaimType == request.ClaimType, cancellationToken);
                if (roleClaim != null) return new CaseResult<bool>(string.Equals(roleClaim.ClaimValue, request.AcceptedValue, StringComparison.OrdinalIgnoreCase), (string) null);

                return new CaseResult<bool>(false);
            }
            catch (Exception e)
            {
                return new CaseResult<bool>(e);
            }
        }
    }
}