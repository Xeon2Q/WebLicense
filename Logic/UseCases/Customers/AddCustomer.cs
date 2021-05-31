using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class AddCustomer : IRequest<CaseResult<Customer>>
    {
        internal string Name { get; }
        internal string Email { get; }
        internal long UserId { get; }
        internal long AdminId { get; }

        public AddCustomer(string name, string email, long userId, long adminId)
        {
            Name = name?.Trim();
            Email = email?.Trim();
            UserId = userId > 0 ? userId : 0;
            AdminId = adminId > 0 ? adminId : 0;
        }
    }

    internal sealed class AddCustomerHandler : IRequestHandler<AddCustomer, CaseResult<Customer>>
    {
        private readonly DatabaseContext db;

        public AddCustomerHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<Customer>> Handle(AddCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = new Customer
                {
                    Name = request.Name,
                    Code = string.Empty.GetRandom(50),
                    ReferenceId = Guid.NewGuid().ToString("N"),
                    Settings = new CustomerSettings {NotificationsEmail = request.Email},
                    CustomerManagers = new List<CustomerManager>{new(){UserId = request.UserId}}
                };
                if (request.AdminId > 0)
                {
                    customer.Administrators = new List<User> {new() {Id = request.AdminId}};
                }

                var result = await db.Customers.AddAsync(customer, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return new CaseResult<Customer>(result.Entity);
            }
            catch (Exception e)
            {
                return new CaseResult<Customer>(e);
            }
        }
    }
}