using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        internal long CurrentUserId { get; }

        public UpdateCompany(CompanyInfo customer, long currentUserId)
        {
            Company = customer;
            CurrentUserId = currentUserId;
        }

        public void Validate()
        {
            if (CurrentUserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'CurrentUserId' < 1");
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

                var access = await sender.Send(new GetCompanyAccess(request.Company.Id ?? 0, request.CurrentUserId), cancellationToken);
                if (!access.HasAccess) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"User({request.CurrentUserId}) does not have permissions to view Company({request.Company.Id})");

                var info = request.Company;
                var model = await db.Set<Company>().AsTracking().Where(q => q.Id == info.Id.Value)
                                    .Include(q => q.Settings)
                                    .Include(q => q.ClientSettings)
                                    .Include(q => q.CompanyUsers).ThenInclude(q => q.User).FirstOrDefaultAsync(cancellationToken);
                if (model == null) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, "Company not found or deleted");

                UpdateModel(model, info, access);

                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCompany(model.Id, request.CurrentUserId), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }

        #region Methods

        private void UpdateModel(Company model, CompanyInfo info, CompanyAccessInfo access)
        {
            if (access.IsManagerAccess)
            {
                UpdateModelCore(model, info);
            }

            if (info.Settings?.ProviderCompanyId != null && (access.IsAdminAccess || access.EditSettingsAllowedId.Contains(info.Settings.ProviderCompanyId.Value)))
            {
                var settings = model.Settings?.FirstOrDefault(q => q.ProviderCompanyId == info.Settings.ProviderCompanyId);
                UpdateModelSettings(settings, info.Settings);
            }

            if (info.Users != null && access.IsManagerAccess)
            {
                UpdateModelUsers(model, info);
                UpdateModelInvites(model, info);
            }
        }

        private void UpdateModelCore(Company model, CompanyInfo info)
        {
            if (info.Name != null && info.Name != model.Name) model.Name = info.Name;
            if (info.Code != null && info.Code != model.Code) model.Code = info.Code;
            if (info.ReferenceId != null && info.ReferenceId != model.ReferenceId) model.ReferenceId = info.ReferenceId;
            if (info.Logo != null && info.Logo != model.Logo) model.Logo = info.Logo;
        }

        private void UpdateModelSettings(CompanySettings model, CompanySettingsInfo info)
        {
            if (model == null || info == null) return;

            if (info.MaxActiveLicensesCount.HasValue && info.MaxActiveLicensesCount != model.MaxActiveLicensesCount) model.MaxActiveLicensesCount = info.MaxActiveLicensesCount.Value;
            if (info.MaxTotalLicensesCount.HasValue && info.MaxTotalLicensesCount != model.MaxTotalLicensesCount) model.MaxTotalLicensesCount = info.MaxTotalLicensesCount.Value;
            if (info.CreateActiveLicenses.HasValue && info.CreateActiveLicenses != model.CreateActiveLicenses) model.CreateActiveLicenses = info.CreateActiveLicenses.Value;
            if (info.CanActivateLicenses.HasValue && info.CanActivateLicenses != model.CanActivateLicenses) model.CanActivateLicenses = info.CanActivateLicenses.Value;
            if (info.CanDeactivateLicenses.HasValue && info.CanDeactivateLicenses != model.CanDeactivateLicenses) model.CanDeactivateLicenses = info.CanDeactivateLicenses.Value;
            if (info.CanDeleteLicenses.HasValue && info.CanDeleteLicenses != model.CanDeleteLicenses) model.CanDeleteLicenses = info.CanDeleteLicenses.Value;
            if (info.CanActivateMachines.HasValue && info.CanActivateMachines != model.CanActivateMachines) model.CanActivateMachines = info.CanActivateMachines.Value;
            if (info.CanDeactivateMachines.HasValue && info.CanDeactivateMachines != model.CanDeactivateMachines) model.CanDeactivateMachines = info.CanDeactivateMachines.Value;
            if (info.CanDeleteMachines.HasValue && info.CanDeleteMachines != model.CanDeleteMachines) model.CanDeleteMachines = info.CanDeleteMachines.Value;
            if (info.NotificationsEmail != null && info.NotificationsEmail != model.NotificationsEmail) model.NotificationsEmail = !string.IsNullOrWhiteSpace(info.NotificationsEmail) ? info.NotificationsEmail.Trim() : null;
            if (info.ReceiveNotifications.HasValue && info.ReceiveNotifications != model.ReceiveNotifications) model.ReceiveNotifications = info.ReceiveNotifications.Value;
        }

        private void UpdateModelUsers(Company model, CompanyInfo info)
        {
            if (info?.Users == null) return;

            model.CompanyUsers ??= new List<CompanyUser>();

            // remove users
            var removed = model.CompanyUsers.Where(q => info.Users.All(w => w.Id != q.UserId)).ToArray();
            if (removed.Any()) db.Set<CompanyUser>().RemoveRange(removed);

            // update users
            foreach (var infoUser in info.Users.Where(q => !q.IsInvite && q.Id.HasValue))
            {
                var modelUser = model.CompanyUsers.FirstOrDefault(q => q.UserId == infoUser.Id);
                if (modelUser == null) continue;

                if (infoUser.IsManager.HasValue) modelUser.IsManager = infoUser.IsManager.Value;
            }

            // add new users functionality is not supported, because new users should be invited
        }

        private void UpdateModelInvites(Company model, CompanyInfo info)
        {
            if (info?.Users == null) return;

            model.CompanyUserInvites ??= new List<CompanyUserInvite>();

            // remove users
            var removed = model.CompanyUserInvites.Where(q => info.Users.Where(w => w.IsInvite).All(w => w.Email != q.Email)).ToArray();
            if (removed.Any()) db.Set<CompanyUserInvite>().RemoveRange(removed);

            // update users
            foreach (var infoUser in info.Users.Where(q => q.IsInvite && q.Id.HasValue))
            {
                var modelUser = model.CompanyUsers.FirstOrDefault(q => q.UserId == infoUser.Id);
                if (modelUser == null) continue;

                if (infoUser.IsManager.HasValue) modelUser.IsManager = infoUser.IsManager.Value;
            }

            // add invites
            var added = info.Users.Where(q => q.IsInvite && !q.Id.HasValue).GroupBy(q => q.Email.ToLower()).Select(q => q.FirstOrDefault(w => w.IsManager == true) ?? q.FirstOrDefault()).ToArray();
            foreach (var infoUser in added)
            {
                var modelUser = model.CompanyUsers.FirstOrDefault(q => q.User != null && string.Equals(q.User.Email, infoUser.Email, StringComparison.OrdinalIgnoreCase));
                if (modelUser != null) continue;

                model.CompanyUserInvites.Add(new CompanyUserInvite{Company = model, Email = infoUser.Email, IsManager = infoUser.IsManager == true, Processed = false});
            }
        }

        #endregion
    }
}