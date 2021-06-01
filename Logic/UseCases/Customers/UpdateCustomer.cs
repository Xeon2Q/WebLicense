using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class UpdateCustomer : IRequest<CaseResult<Customer>>
    {
        internal CustomerInfo Customer { get; }

        public UpdateCustomer(CustomerInfo customer)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        }
    }

    internal sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomer, CaseResult<Customer>>
    {
        public Task<CaseResult<Customer>> Handle(UpdateCustomer request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}