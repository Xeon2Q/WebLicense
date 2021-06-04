using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources;
using WebLicense.Access;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class HasClaim : IRequest<CaseResult<bool>>, IValidate
    {
        internal readonly UserInfo User;
        internal readonly string ClaimType;
        internal readonly string AcceptedValue;

        public HasClaim(UserInfo user, string claimType, string acceptedValue)
        {
            User = user;
            ClaimType = claimType;
            AcceptedValue = acceptedValue;
        }

        public void Validate()
        {
            if (User == null) throw new CaseException(Exceptions.User_Null, "'User' is null");
            if (!User.Id.HasValue) throw new CaseException(Exceptions.User_Id_Null, "User 'Id' is null");
            if (User.Id < 1) throw new CaseException(Exceptions.User_Id_LessOne, "User 'Id' < 1");
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
                request.Validate();

                if (string.IsNullOrWhiteSpace(request.ClaimType)) return new CaseResult<bool>(false, (string) null);

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