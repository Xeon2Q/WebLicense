using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class UpdateCompany : IRequest<CaseResult<CompanyInfo>>, IValidate
    {
        internal CompanyInfo Company { get; }
        internal long UserId { get; }

        public UpdateCompany(CompanyInfo customer, long userId)
        {
            Company = customer;
            UserId = userId;
        }

        public void Validate()
        {
            if (UserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'UserId' < 1");
            if (Company == null) throw new CaseException(Exceptions.Company_Null, "Request is null");
            if (!Company.Id.HasValue) throw new CaseException(Exceptions.Company_Id_Null, "Company 'Id' is null");
            if (Company.Id < 1) throw new CaseException(Exceptions.Company_Id_LessOne, "Company 'Id' < 1");
        }
    }

    internal sealed class UpdateCompanyHandler : IRequestHandler<UpdateCompany, CaseResult<CompanyInfo>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public UpdateCompanyHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<CompanyInfo>> Handle(UpdateCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var info = request.Company;
                var model = await db.Set<Company>().AsTracking().Where(q => q.Id == info.Id.Value)
                                    .Include(q => q.Settings)
                                    .Include(q => q.ClientSettings)
                                    .Include(q => q.CompanyUsers).FirstOrDefaultAsync(cancellationToken);
                if (model == null) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, "Company not found or deleted");

                UpdateModel(model, info);

                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCompany(model.Id, request.UserId), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }

        #region Methods

        private void UpdateModel(Company model, CompanyInfo info)
        {
            if (info.Name != null && info.Name != model.Name) model.Name = info.Name;
            if (info.Code != null && info.Code != model.Code) model.Code = info.Code;
            if (info.ReferenceId != null && info.ReferenceId != model.ReferenceId) model.ReferenceId = info.ReferenceId;
            if (info.Logo != null && info.Logo != model.Logo) model.Logo = info.Logo;

            var settings = model.Settings?.FirstOrDefault(q => q.ProviderCompanyId == info.Settings?.ProviderCompanyId);
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

            if (info.Users != null)
            {
                model.CompanyUsers = model.CompanyUsers?.Where(q => info.Users.Any(w => w.Id == q.UserId)).ToList() ?? new List<CompanyUser>();
                
                GetNewUsers(info.Users, model.CompanyUsers.Select(q => q.UserId)).ForEach(q => model.CompanyUsers.Add(new CompanyUser {CompanyId = model.Id, UserId = q}));
            }
        }

        private List<long> GetNewUsers(IEnumerable<CompanyUserInfo> changedUsers, IEnumerable<long> existingUsers)
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