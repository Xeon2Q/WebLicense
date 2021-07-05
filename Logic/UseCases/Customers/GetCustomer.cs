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
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class GetCustomer : IRequest<CaseResult<CustomerInfo>>, IValidate
    {
        internal int Id { get; }

        public GetCustomer(int id)
        {
            Id = id;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
        }
    }

    internal sealed class GetCustomerHandler : IRequestHandler<GetCustomer, CaseResult<CustomerInfo>>
    {
        private readonly DatabaseContext db;

        public GetCustomerHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<CustomerInfo>> Handle(GetCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var customer = await db.Customers.AsNoTrackingWithIdentityResolution().Where(q => q.Id == request.Id)
                                       .Include(q => q.Companies)
                                       .Include(q => q.Managers)
                                       .Include(q => q.Users)
                                       .Include(q => q.Settings)
                                       .FirstOrDefaultAsync(cancellationToken);

                if (customer == null) throw new CaseException(Exceptions.Customer_NotFoundOrDeleted, $"Customer({request.Id}) not found or deleted");

                return new CaseResult<CustomerInfo>(new CustomerInfo(customer, null));
            }
            catch (Exception e)
            {
                return new CaseResult<CustomerInfo>(e);
            }
        }
    }
}