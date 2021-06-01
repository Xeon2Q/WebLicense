using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
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
            Customer = customer ?? throw new ArgumentNullException(nameof(customer), "'customer' cannot be null");

            if (!Customer.Id.HasValue) throw new ArgumentNullException(nameof(customer), "'customer.Id' cannot be null");
        }
    }

    internal sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomer, CaseResult<Customer>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public UpdateCustomerHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<Customer>> Handle(UpdateCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                var info = request.Customer;
                var model = await db.Set<Customer>().AsTracking().Where(q => q.Id == info.Id.Value)
                                    .Include(q => q.Settings)
                                    .Include(q => q.CustomerAdministrators)
                                    .Include(q => q.CustomerManagers)
                                    .Include(q => q.CustomerUsers).FirstOrDefaultAsync(cancellationToken);
                if (model == null) throw new Exception("Customer not found or deleted");

                UpdateModel(model, info);

                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCustomer(model.Id), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<Customer>(e);
            }
        }

        #region Methods

        private void UpdateModel(Customer model, CustomerInfo info)
        {
            if (info.Name != null && info.Name != model.Name) model.Name = info.Name;
            if (info.Code != null && info.Code != model.Code) model.Code = info.Code;
            if (info.ReferenceId != null && info.ReferenceId != model.ReferenceId) model.ReferenceId = info.ReferenceId;

            if (info.Settings != null)
            {
                if (info.Settings.MaxActiveLicensesCount.HasValue && info.Settings.MaxActiveLicensesCount != model.Settings.MaxActiveLicensesCount) model.Settings.MaxActiveLicensesCount = info.Settings.MaxActiveLicensesCount.Value;
                if (info.Settings.MaxTotalLicensesCount.HasValue && info.Settings.MaxTotalLicensesCount != model.Settings.MaxTotalLicensesCount) model.Settings.MaxTotalLicensesCount = info.Settings.MaxTotalLicensesCount.Value;
                if (info.Settings.CreateActiveLicenses.HasValue && info.Settings.CreateActiveLicenses != model.Settings.CreateActiveLicenses) model.Settings.CreateActiveLicenses = info.Settings.CreateActiveLicenses.Value;
                if (info.Settings.CanActivateLicenses.HasValue && info.Settings.CanActivateLicenses != model.Settings.CanActivateLicenses) model.Settings.CanActivateLicenses = info.Settings.CanActivateLicenses.Value;
                if (info.Settings.CanDeactivateLicenses.HasValue && info.Settings.CanDeactivateLicenses != model.Settings.CanDeactivateLicenses) model.Settings.CanDeactivateLicenses = info.Settings.CanDeactivateLicenses.Value;
                if (info.Settings.CanDeleteLicenses.HasValue && info.Settings.CanDeleteLicenses != model.Settings.CanDeleteLicenses) model.Settings.CanDeleteLicenses = info.Settings.CanDeleteLicenses.Value;
                if (info.Settings.CanActivateMachines.HasValue && info.Settings.CanActivateMachines != model.Settings.CanActivateMachines) model.Settings.CanActivateMachines = info.Settings.CanActivateMachines.Value;
                if (info.Settings.CanDeactivateMachines.HasValue && info.Settings.CanDeactivateMachines != model.Settings.CanDeactivateMachines) model.Settings.CanDeactivateMachines = info.Settings.CanDeactivateMachines.Value;
                if (info.Settings.CanDeleteMachines.HasValue && info.Settings.CanDeleteMachines != model.Settings.CanDeleteMachines) model.Settings.CanDeleteMachines = info.Settings.CanDeleteMachines.Value;
                if (info.Settings.NotificationsEmail != null && info.Settings.NotificationsEmail != model.Settings.NotificationsEmail) model.Settings.NotificationsEmail = !string.IsNullOrWhiteSpace(info.Settings.NotificationsEmail) ? info.Settings.NotificationsEmail.Trim() : null;
                if (info.Settings.ReceiveNotifications.HasValue && info.Settings.ReceiveNotifications != model.Settings.ReceiveNotifications) model.Settings.ReceiveNotifications = info.Settings.ReceiveNotifications.Value;
            }

            if (info.Administrators != null)
            {
                model.CustomerAdministrators ??= new List<CustomerAdministrator>();
                model.CustomerAdministrators = model.CustomerAdministrators.Where(q => info.Administrators.Any(w => w.Id == q.UserId)).ToList();
                var newItems = info.Administrators.Where(q => q.Id.HasValue && model.CustomerAdministrators.All(w => w.UserId != q.Id)).ToList();
                foreach (var newItem in newItems)
                {
                    model.CustomerAdministrators.Add(new CustomerAdministrator {CustomerId = model.Id, UserId = newItem.Id.Value});
                }
            }

            if (info.Managers != null)
            {
                model.CustomerManagers ??= new List<CustomerManager>();
                model.CustomerManagers = model.CustomerManagers.Where(q => info.Managers.Any(w => w.Id == q.UserId)).ToList();
                var newItems = info.Managers.Where(q => q.Id.HasValue && model.CustomerManagers.All(w => w.UserId != q.Id)).ToList();
                foreach (var newItem in newItems)
                {
                    model.CustomerManagers.Add(new CustomerManager {CustomerId = model.Id, UserId = newItem.Id.Value});
                }
            }

            if (info.Users != null)
            {
                model.CustomerUsers ??= new List<CustomerUser>();
                model.CustomerUsers = model.CustomerUsers.Where(q => info.Users.Any(w => w.Id == q.UserId)).ToList();
                var newItems = info.Users.Where(q => q.Id.HasValue && model.CustomerUsers.All(w => w.UserId != q.Id)).ToList();
                foreach (var newItem in newItems)
                {
                    model.CustomerUsers.Add(new CustomerUser {CustomerId = model.Id, UserId = newItem.Id.Value});
                }
            }
        }

        #endregion
    }
}