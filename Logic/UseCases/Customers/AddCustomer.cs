using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class AddCustomer : IRequest<CaseResult<CustomerInfo>>
    {
        internal CustomerInfo Customer { get; }

        public AddCustomer(CustomerInfo customer)
        {
            Customer = customer;
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
                ValidateRequest(request);

                var users = request.Customer.Users.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CustomerUser {UserId = q}).ToList();
                var managers = request.Customer.Managers?.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CustomerManager {UserId = q}).ToList() ?? new List<CustomerManager>();
                var administrators = request.Customer.Administrators?.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CustomerAdministrator {UserId = q}).ToList() ?? new List<CustomerAdministrator>(0);

                if (managers.Count == 0) managers = new List<CustomerManager> {new() {UserId = users.First().UserId}};

                var customer = new Customer
                {
                    Name = request.Customer.Name,
                    Code = string.Empty.GetRandom(50),
                    ReferenceId = Guid.NewGuid().ToString("N"),
                    Settings = new CustomerSettings {NotificationsEmail = request.Customer.Settings.NotificationsEmail},
                    CustomerUsers = users,
                    CustomerManagers = managers,
                    CustomerAdministrators = administrators
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

        #region Methods

        private void ValidateRequest(AddCustomer request)
        {
            if (request?.Customer == null) throw new CaseException("*Request is null", "Request is null");
            if (string.IsNullOrWhiteSpace(request.Customer.Settings?.NotificationsEmail)) throw new CaseException("*'Email' cannot be null", "'Email' is null");
            if (string.IsNullOrWhiteSpace(request.Customer.Name)) throw new CaseException("*'Name' cannot be null", "'Name is null'");
            if (request.Customer.Users == null || request.Customer.Users.Count == 0) throw new CaseException("*'Users' cannot be null or empty", "'Users' is null or empty");
            if (request.Customer.Users.All(q => q == null || !q.Id.HasValue || q.Id < 1)) throw new CaseException("*'Users' must have 'Id' > 0", "'Users' have 'Id' < 1");
        }

        #endregion
    }
}