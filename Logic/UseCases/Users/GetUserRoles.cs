using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetUserRoles : IRequest<CaseResult<IList<Role>>>, IValidate
    {
        internal long UserId { get; }

        public GetUserRoles(long userId)
        {
            UserId = userId;
        }

        public void Validate()
        {
            if (UserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'UserId' < 1");
        }
    }

    internal sealed class GetUserRolesHandler : IRequestHandler<GetUserRoles, CaseResult<IList<Role>>>
    {
        private readonly DatabaseContext db;

        public GetUserRolesHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<IList<Role>>> Handle(GetUserRoles request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var roles = await db.UserRoles.AsNoTrackingWithIdentityResolution().Where(q => q.UserId == request.UserId).Select(q => q.Role).Distinct().ToListAsync(cancellationToken);

                return new CaseResult<IList<Role>>(roles);
            }
            catch (Exception e)
            {
                return new CaseResult<IList<Role>>(e);
            }
        }
    }
}