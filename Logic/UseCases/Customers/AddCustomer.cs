using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class AddCustomer : IRequest<CaseResult<CustomerInfo>>, IValidate
    {
        internal CustomerInfo Customer { get; }

        public AddCustomer(CustomerInfo customer)
        {
            Customer = customer;
        }

        public void Validate()
        {
            if (!(Customer.Settings?.CompanyId > 0)) throw new CaseException(Exceptions.Customer_CompanyId_LessOne, "Customer 'Settings.CompanyId' < 1");
            if (string.IsNullOrWhiteSpace(Customer.Settings?.NotificationsEmail)) throw new CaseException(Exceptions.Customer_NotificationsEmail_Empty, "Customer 'NotificationsEmail' is empty");
            if (string.IsNullOrWhiteSpace(Customer.Name)) throw new CaseException(Exceptions.Customer_Name_Empty, "Customer 'Name' is empty");
            if (Customer.Users == null || Customer.Users.Count == 0) throw new CaseException(Exceptions.Customer_Users_Empty, "Customer 'Users' is empty");
            if (Customer.Users.All(q => q == null || !q.Id.HasValue || q.Id < 1)) throw new CaseException(Exceptions.Customer_Users_Invalid, "Customer 'Users' are invalid");
        }
    }

    internal sealed class AddCustomerHandler : IRequestHandler<AddCustomer, CaseResult<CustomerInfo>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public AddCustomerHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<CustomerInfo>> Handle(AddCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var users = request.Customer.Users.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CustomerUser {UserId = q}).ToList();
                var managers = request.Customer.Managers?.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CustomerManager {UserId = q}).ToList() ?? new List<CustomerManager>();

                if (managers.Count == 0) managers = new List<CustomerManager> {new() {UserId = users.First().UserId}};

                var customer = new Customer
                {
                    Name = request.Customer.Name,
                    Code = string.Empty.GetRandom(50),
                    ReferenceId = Guid.NewGuid().ToString("N"),
                    Settings = new List<CustomerSettings> {new()
                    {
                        CompanyId = request.Customer.Settings.CompanyId ?? -1,
                        NotificationsEmail = request.Customer.Settings.NotificationsEmail
                    }},
                    CustomerUsers = users,
                    CustomerManagers = managers
                };

                var result = await db.Customers.AddAsync(customer, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCustomer(result.Entity.Id), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CustomerInfo>(e);
            }
        }
    }
}