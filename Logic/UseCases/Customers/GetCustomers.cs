using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class GetCustomers : IRequest<CaseResult<IList<CustomerInfo>>>
    {
        internal Criteria<Customer> Criteria { get; }

        public GetCustomers(Criteria<Customer> criteria)
        {
            Criteria = criteria;
        }

        public GetCustomers(int skip, int take, string sort, bool sortAsc, Expression<Func<Customer, bool>> filter) : this(new Criteria<Customer>(skip, take, sort, sortAsc, filter))
        {
        }

        public GetCustomers() : this(0, 25, nameof(Customer.Name), true, null)
        {
        }
    }

    internal sealed class GetCustomersHandler : IRequestHandler<GetCustomers, CaseResult<IList<CustomerInfo>>>
    {
        private readonly DatabaseContext db;

        public GetCustomersHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<IList<CustomerInfo>>> Handle(GetCustomers request, CancellationToken cancellationToken)
        {
            try
            {
                var query = db.Set<Customer>().AsNoTrackingWithIdentityResolution();
                query = ApplyFilter(query, request.Criteria);
                query = ApplySort(query, request.Criteria);
                query = ApplySkipTake(query, request.Criteria);

                var data1 = await query.ToListAsync(cancellationToken);
                var data2 = data1.Select(q => new CustomerInfo(q)).ToList();

                return new CaseResult<IList<CustomerInfo>>(data2);

            }
            catch (Exception e)
            {
                return new CaseResult<IList<CustomerInfo>>(e);
            }
        }

        #region Methods

        private IQueryable<Customer> ApplyFilter(IQueryable<Customer> query, Criteria<Customer> criteria)
        {
            return criteria?.Filter != null ? query.Where(criteria.Filter) : query;
        }

        private IQueryable<Customer> ApplySort(IQueryable<Customer> query, Criteria<Customer> criteria)
        {
            if (criteria == null || string.IsNullOrWhiteSpace(criteria.Sort)) return query;

            return (criteria.SortAsc, criteria.Sort.ToUpper()) switch
            {
                (true, "ID") => query.OrderBy(q => q.Id),
                (false, "ID") => query.OrderByDescending(q => q.Id),

                (true, "NAME") => query.OrderBy(q => q.Name),
                (false, "NAME") => query.OrderByDescending(q => q.Name),

                _ => query.OrderBy(q => q.Name)
            };
        }

        private IQueryable<Customer> ApplySkipTake(IQueryable<Customer> query, Criteria<Customer> criteria)
        {
            return criteria != null ? query.Skip(criteria.Skip).Take(criteria.Take) : query;
        }

        #endregion
    }
}