using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Companies;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Logic.UseCases.Users;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class GetCompany : IRequest<CaseResult<CompanyInfo>>, IValidate
    {
        internal int Id { get; }
        internal long UserId { get; }

        public GetCompany(int id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
            if (UserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'UserId' < 1");
        }
    }

    internal sealed class GetCompanyHandler : IRequestHandler<GetCompany, CaseResult<CompanyInfo>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public GetCompanyHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<CompanyInfo>> Handle(GetCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var roles = await GetUserRoles(request.UserId, cancellationToken);
                if (!roles.Any()) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"User({request.UserId}) does not have permissions to view Company({request.Id})");

                var company = await db.Companies.AsNoTrackingWithIdentityResolution().Where(q => q.Id == request.Id)
                                      .Include(q => q.Users)
                                      .Include(q => q.CompanyUserInvites)
                                      .FirstOrDefaultAsync(cancellationToken);
                if (company == null) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"Company({request.Id}) not found or deleted");

                company.Settings = await GetAvailableCompanySettings(company, request.UserId, roles, cancellationToken);

                return new CaseResult<CompanyInfo>(new CompanyInfo(company));
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }

        #region Methods

        private async Task<IList<Role>> GetUserRoles(long userId, CancellationToken cancellationToken)
        {
            var roles = await sender.Send(new GetUserRoles(userId), cancellationToken);
            roles.ThrowOnFail();

            return roles.Data ?? new List<Role>(0);
        }

        private async Task<IList<CompanySettings>> GetAvailableCompanySettings(Company company, long userId, IList<Role> roles, CancellationToken cancellationToken)
        {
            if (company == null || company.Id < 1 || roles == null || !roles.Any()) return null;

            // global admin role
            if (roles.Any(q => q.Id == Roles.AdminId))
            {
                return await GetAllCompanySettings(company.Id, cancellationToken);
            }

            // user is in the company
            if (company.Users != null && company.Users.Any(q => q.Id == userId))
            {
                return await GetAllCompanySettings(company.Id, cancellationToken);
            }

            // user is from provider company (-s)
            return await GetProviderCompanySettingsByUserId(company.Id, userId, cancellationToken);
        }

        private async Task<IList<CompanySettings>> GetAllCompanySettings(int id, CancellationToken cancellationToken)
        {
            return await db.Set<CompanySettings>().AsNoTrackingWithIdentityResolution().Where(q => q.CompanyId == id).Distinct().ToListAsync(cancellationToken);
        }

        private async Task<IList<CompanySettings>> GetProviderCompanySettingsByUserId(int id, long userId, CancellationToken cancellationToken)
        {
            return await db.Set<CompanyUser>().AsNoTrackingWithIdentityResolution()
                           .Where(q => q.UserId == userId)
                           .SelectMany(q => q.Company.ClientSettings)
                           .Where(q => q.CompanyId == id)
                           .Distinct().ToListAsync(cancellationToken);
        }

        #endregion
    }
}