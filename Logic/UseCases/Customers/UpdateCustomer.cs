using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Customers;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class UpdateCustomer : IRequest<CaseResult<CustomerInfo>>, IValidate
    {
        internal CustomerInfo Customer { get; }

        public UpdateCustomer(CustomerInfo customer)
        {
            Customer = customer;
        }

        public void Validate()
        {
            if (Customer == null) throw new CaseException(Exceptions.Customer_Null, "Request is null");
            if (!Customer.Id.HasValue) throw new CaseException(Exceptions.Customer_Id_Null, "Customer 'Id' is null");
            if (Customer.Id < 1) throw new CaseException(Exceptions.Customer_Id_LessOne, "Customer 'Id' < 1");
        }
    }

    internal sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomer, CaseResult<CustomerInfo>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public UpdateCustomerHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<CustomerInfo>> Handle(UpdateCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var info = request.Customer;
                var model = await db.Set<Customer>().AsTracking().Where(q => q.Id == info.Id.Value)
                                    .Include(q => q.Settings)
                                    .Include(q => q.Companies)
                                    .Include(q => q.CustomerManagers)
                                    .Include(q => q.CustomerUsers).FirstOrDefaultAsync(cancellationToken);
                if (model == null) throw new CaseException(Exceptions.Customer_NotFoundOrDeleted, "Customer not found or deleted");

                UpdateModel(model, info);

                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCustomer(model.Id), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CustomerInfo>(e);
            }
        }

        #region Methods

        private void UpdateModel(Customer model, CustomerInfo info)
        {
            if (info.Name != null && info.Name != model.Name) model.Name = info.Name;
            if (info.Code != null && info.Code != model.Code) model.Code = info.Code;
            if (info.ReferenceId != null && info.ReferenceId != model.ReferenceId) model.ReferenceId = info.ReferenceId;

            var settings = model.Settings?.FirstOrDefault(q => q.CompanyId == info.Settings?.CompanyId);
            if (settings != null)
            {
                if (info.Settings.MaxActiveLicensesCount.HasValue && info.Settings.MaxActiveLicensesCount != settings.MaxActiveLicensesCount) settings.MaxActiveLicensesCount = info.Settings.MaxActiveLicensesCount.Value;
                if (info.Settings.MaxTotalLicensesCount.HasValue && info.Settings.MaxTotalLicensesCount != settings.MaxTotalLicensesCount) settings.MaxTotalLicensesCount = info.Settings.MaxTotalLicensesCount.Value;
                if (info.Settings.CreateActiveLicenses.HasValue && info.Settings.CreateActiveLicenses != settings.CreateActiveLicenses) settings.CreateActiveLicenses = info.Settings.CreateActiveLicenses.Value;
                if (info.Settings.CanActivateLicenses.HasValue && info.Settings.CanActivateLicenses != settings.CanActivateLicenses) settings.CanActivateLicenses = info.Settings.CanActivateLicenses.Value;
                if (info.Settings.CanDeactivateLicenses.HasValue && info.Settings.CanDeactivateLicenses != settings.CanDeactivateLicenses) settings.CanDeactivateLicenses = info.Settings.CanDeactivateLicenses.Value;
                if (info.Settings.CanDeleteLicenses.HasValue && info.Settings.CanDeleteLicenses != settings.CanDeleteLicenses) settings.CanDeleteLicenses = info.Settings.CanDeleteLicenses.Value;
                if (info.Settings.CanActivateMachines.HasValue && info.Settings.CanActivateMachines != settings.CanActivateMachines) settings.CanActivateMachines = info.Settings.CanActivateMachines.Value;
                if (info.Settings.CanDeactivateMachines.HasValue && info.Settings.CanDeactivateMachines != settings.CanDeactivateMachines) settings.CanDeactivateMachines = info.Settings.CanDeactivateMachines.Value;
                if (info.Settings.CanDeleteMachines.HasValue && info.Settings.CanDeleteMachines != settings.CanDeleteMachines) settings.CanDeleteMachines = info.Settings.CanDeleteMachines.Value;
                if (info.Settings.NotificationsEmail != null && info.Settings.NotificationsEmail != settings.NotificationsEmail) settings.NotificationsEmail = !string.IsNullOrWhiteSpace(info.Settings.NotificationsEmail) ? info.Settings.NotificationsEmail.Trim() : null;
                if (info.Settings.ReceiveNotifications.HasValue && info.Settings.ReceiveNotifications != settings.ReceiveNotifications) settings.ReceiveNotifications = info.Settings.ReceiveNotifications.Value;
            }

            if (info.Managers != null)
            {
                model.CustomerManagers ??= new List<CustomerManager>();
                model.CustomerManagers = model.CustomerManagers.Where(q => info.Managers.Any(w => w.Id == q.UserId)).ToList();

                GetNewUsers(info.Managers, model.CustomerManagers.Select(q => q.UserId)).ForEach(q => model.CustomerManagers.Add(new CustomerManager {CustomerId = model.Id, UserId = q}));
            }

            if (info.Users != null)
            {
                model.CustomerUsers ??= new List<CustomerUser>();
                model.CustomerUsers = model.CustomerUsers.Where(q => info.Users.Any(w => w.Id == q.UserId)).ToList();
                
                GetNewUsers(info.Users, model.CustomerUsers.Select(q => q.UserId)).ForEach(q => model.CustomerUsers.Add(new CustomerUser {CustomerId = model.Id, UserId = q}));
            }
        }

        private List<long> GetNewUsers(IEnumerable<CustomerUserInfo> changedUsers, IEnumerable<long> existingUsers)
        {
            var cId = changedUsers.Where(q => q.Id.HasValue).Select(q => q.Id.Value).Distinct().ToList();
            if (!cId.Any()) return new List<long>(0);

            var eId = existingUsers.ToList();
            if (!eId.Any()) return cId;

            return cId.Where(q => !eId.Contains(q)).ToList();
        }

        #endregion
    }
}