using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetUsers : IRequest<CaseResult<IList<UserInfo>>>, IValidate
    {
        internal Criteria<User> Criteria { get; }

        public GetUsers(Criteria<User> criteria)
        {
            Criteria = criteria;
        }

        public GetUsers(int skip, int take, string sort, bool sortAsc, Expression<Func<User, bool>> filter) : this(new Criteria<User>(skip, take, sort, sortAsc, filter))
        {
        }

        public GetUsers() : this(new Criteria<User>(0, 25, nameof(User.UserName), true, null))
        {
        }

        public void Validate()
        {
            if (Criteria == null) throw new CaseException(Exceptions.Criteria_Null, "'Criteria' is null");
        }
    }

    internal sealed class GetUsersHandler : IRequestHandler<GetUsers, CaseResult<IList<UserInfo>>>
    {
        private readonly DatabaseContext db;

        public GetUsersHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<IList<UserInfo>>> Handle(GetUsers request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var query = db.Set<User>().AsNoTrackingWithIdentityResolution();
                request.Criteria?.ApplyAll(ref query);

                var data1 = await query.ToListAsync(cancellationToken);
                var data2 = data1.Select(q => new UserInfo(q)).ToList();

                return new CaseResult<IList<UserInfo>>(data2);

            }
            catch (Exception e)
            {
                return new CaseResult<IList<UserInfo>>(e);
            }
        }
    }
}