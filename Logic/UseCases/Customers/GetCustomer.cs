using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class GetCustomer : IRequest<CaseResult<Customer>>
    {
        internal int Id { get; }

        public GetCustomer(int id)
        {
            Id = id > 0 ? id : throw new ArgumentOutOfRangeException(nameof(id), "'id' must be greater than 0");
        }
    }

    internal sealed class GetCustomerHandler : IRequestHandler<GetCustomer, CaseResult<Customer>>
    {
        private readonly DatabaseContext db;

        public GetCustomerHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<Customer>> Handle(GetCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await db.Customers.Where(q => q.Id == request.Id)
                                       .Include(q => q.Administrators)
                                       .Include(q => q.Managers)
                                       .Include(q => q.Users)
                                       .Include(q => q.Settings)
                                       .FirstOrDefaultAsync(cancellationToken);
                return new CaseResult<Customer>(customer);
            }
            catch (Exception e)
            {
                return new CaseResult<Customer>(e);
            }
        }
    }
}