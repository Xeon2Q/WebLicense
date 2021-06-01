using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
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

        public AddCustomer(string name, string email, long userId, long adminId = 0)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentNullException(nameof(name), "'Name' cannot be null");
            Email = !string.IsNullOrWhiteSpace(email) ? email.Trim() : throw new ArgumentNullException(nameof(name), "'Email' cannot be null");
            UserId = userId > 0 ? userId : throw new ArgumentOutOfRangeException(nameof(userId), "'UserId' must be greater than 0");
            AdminId = adminId > 0 ? adminId : 0;
        }
    }

    internal sealed class AddCustomerHandler : IRequestHandler<AddCustomer, CaseResult<Customer>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public AddCustomerHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
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
                    CustomerUsers = new List<CustomerUser> {new() {UserId = request.UserId}},
                    CustomerManagers = new List<CustomerManager> {new() {UserId = request.UserId}},
                    CustomerAdministrators = request.AdminId > 0 ? new List<CustomerAdministrator> {new() {UserId = request.AdminId}} : new List<CustomerAdministrator>()
                };

                var result = await db.Customers.AddAsync(customer, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCustomer(result.Entity.Id), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<Customer>(e);
            }
        }
    }
}