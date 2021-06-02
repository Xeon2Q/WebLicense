using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Logic.Auxiliary;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class GetCustomer : IRequest<CaseResult<CustomerInfo>>
    {
        internal int Id { get; }

        public GetCustomer(int id)
        {
            Id = id;
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
                ValidateRequest(request);

                var customer = await db.Customers.AsNoTrackingWithIdentityResolution().Where(q => q.Id == request.Id)
                                       .Include(q => q.Administrators)
                                       .Include(q => q.Managers)
                                       .Include(q => q.Users)
                                       .Include(q => q.Settings)
                                       .FirstOrDefaultAsync(cancellationToken);
                return new CaseResult<CustomerInfo>(new CustomerInfo(customer));
            }
            catch (Exception e)
            {
                return new CaseResult<CustomerInfo>(e);
            }
        }

        #region Methods

        private void ValidateRequest(GetCustomer request)
        {
            if (request == null) throw new CaseException("*Request is null", "Request is null");
            if (request.Id < 1) throw new CaseException("*'Id' must be greater than 0", "'Id' < 0");
        }

        #endregion
    }
}