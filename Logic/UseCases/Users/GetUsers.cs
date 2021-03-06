using MediatR;
using Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class GetUsers : IRequest<CaseResult<ListData<UserInfo>>>, IValidate
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

    internal sealed class GetUsersHandler : IRequestHandler<GetUsers, CaseResult<ListData<UserInfo>>>
    {
        private readonly DatabaseContext db;

        public GetUsersHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<ListData<UserInfo>>> Handle(GetUsers request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var total = await request.Criteria.GetTotal(db, cancellationToken);
                var totalFiltered = await request.Criteria.GetTotalFiltered(db, total, cancellationToken);
                var data = (await request.Criteria.GetData(db, cancellationToken)).Select(q => new UserInfo(q)).ToList();

                return new CaseResult<ListData<UserInfo>>(new ListData<UserInfo>(total, totalFiltered, data));

            }
            catch (Exception e)
            {
                return new CaseResult<ListData<UserInfo>>(e);
            }
        }
    }
}