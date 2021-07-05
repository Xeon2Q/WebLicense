using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class GetCustomers : IRequest<CaseResult<ListData<CustomerInfo>>>, IValidate
    {
        internal Criteria<Customer> Criteria { get; }

        public GetCustomers(Criteria<Customer> criteria)
        {
            Criteria = criteria;
        }

        public GetCustomers(int skip, int take, string sort, bool sortAsc, Expression<Func<Customer, bool>> filter) : this(new Criteria<Customer>(skip, take, sort, sortAsc, filter))
        {
        }

        public GetCustomers() : this(new Criteria<Customer>(0, 25, nameof(Customer.Name), true, null))
        {
        }

        public void Validate()
        {
            if (Criteria == null) throw new CaseException(Exceptions.Criteria_Null, "'Criteria' is null");
        }
    }

    internal sealed class GetCustomersHandler : IRequestHandler<GetCustomers, CaseResult<ListData<CustomerInfo>>>
    {
        private readonly DatabaseContext db;

        public GetCustomersHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<ListData<CustomerInfo>>> Handle(GetCustomers request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var query = db.Set<Customer>().AsNoTrackingWithIdentityResolution();
                request.Criteria?.ApplyAll(ref query);

                var data1 = await query.ToListAsync(cancellationToken);
                var data2 = data1.Select(q => new CustomerInfo(q, null)).ToList();

                var total = await db.Set<Customer>().CountAsync(cancellationToken);

                return new CaseResult<ListData<CustomerInfo>>(new ListData<CustomerInfo>(total, data2));

            }
            catch (Exception e)
            {
                return new CaseResult<ListData<CustomerInfo>>(e);
            }
        }
    }
}