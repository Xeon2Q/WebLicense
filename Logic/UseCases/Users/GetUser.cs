using System;
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
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetUser : IRequest<CaseResult<UserInfo>>, IValidate
    {
        internal long Id { get; }

        public GetUser(long id)
        {
            Id = id;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
        }
    }

    internal sealed class GetUserHandler : IRequestHandler<GetUser, CaseResult<UserInfo>>
    {
        private readonly DatabaseContext db;

        public GetUserHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<UserInfo>> Handle(GetUser request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var user = await db.Set<User>().Where(q => q.Id == request.Id)
                                   .Include(q => q.Companies)
                                   .FirstOrDefaultAsync(cancellationToken);

                if (user == null) throw new CaseException("*User not found or deleted", $"User ('{request.Id}') not found or deleted");

                return new(new UserInfo(user));
            }
            catch (Exception e)
            {
                return new(e);
            }
        }
    }
}