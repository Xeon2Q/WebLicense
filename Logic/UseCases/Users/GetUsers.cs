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
        private const string SORT_ID = "ID";
        private const string SORT_NAME = "USERNAME";

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
                query = ApplyFilter(query, request.Criteria);
                query = ApplySort(query, request.Criteria);
                query = ApplySkipTake(query, request.Criteria);

                var data1 = await query.ToListAsync(cancellationToken);
                var data2 = data1.Select(q => new UserInfo(q)).ToList();

                return new CaseResult<IList<UserInfo>>(data2);

            }
            catch (Exception e)
            {
                return new CaseResult<IList<UserInfo>>(e);
            }
        }

        #region Methods

        private IQueryable<User> ApplyFilter(IQueryable<User> query, Criteria<User> criteria)
        {
            return criteria.Filter != null ? query.Where(criteria.Filter) : query;
        }

        private IQueryable<User> ApplySort(IQueryable<User> query, Criteria<User> criteria)
        {
            return (criteria.SortAsc, criteria.Sort?.ToUpper()) switch
            {
                (true, SORT_ID) => query.OrderBy(q => q.Id),
                (false, SORT_ID) => query.OrderByDescending(q => q.Id),

                (true, SORT_NAME) => query.OrderBy(q => q.UserName),
                (false, SORT_NAME) => query.OrderByDescending(q => q.UserName),

                _ => query.OrderBy(q => q.UserName)
            };
        }

        private IQueryable<User> ApplySkipTake(IQueryable<User> query, Criteria<User> criteria)
        {
            return query.Skip(criteria.Skip).Take(criteria.Take);
        }

        #endregion
    }
}