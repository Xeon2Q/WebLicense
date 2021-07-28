using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
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
    public sealed class GetCompany : IRequest<CaseResult<CompanyInfo>>, IValidate
    {
        internal int Id { get; }
        internal long CurrentUserId { get; }

        public GetCompany(int id, long currentUserId)
        {
            Id = id;
            CurrentUserId = currentUserId;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
            if (CurrentUserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'CurrentUserId' < 1");
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

                var access = await sender.Send(new GetCompanyAccess(request.Id, request.CurrentUserId), cancellationToken);
                if (!access.HasAccess) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"User({request.CurrentUserId}) does not have permissions to view Company({request.Id})");

                var company = access.IsAdminAccess || access.IsManagerAccess || access.IsUserAccess
                    ? await GetCompanyFullInformation(request.Id, cancellationToken)
                    : await GetCompanyPartialInformation(request.Id, access, cancellationToken);

                if (company == null) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"Company({request.Id}) not found or deleted");

                return new CaseResult<CompanyInfo>(new CompanyInfo(company));
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }

        #region Methods

        private async Task<Company> GetCompanyFullInformation(int companyId, CancellationToken cancellationToken)
        {
            return await db.Companies.AsNoTrackingWithIdentityResolution().AsSplitQuery().Where(q => q.Id == companyId)
                           .Include(q => q.Users)
                           .Include(q => q.Settings).ThenInclude(q => q.ProviderCompany)
                           .Include(q => q.CompanyUsers)
                           .Include(q => q.CompanyUserInvites)
                           .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<Company> GetCompanyPartialInformation(int companyId, CompanyAccessInfo access, CancellationToken cancellationToken)
        {
            var company = await db.Companies.AsNoTrackingWithIdentityResolution().Where(q => q.Id == companyId).FirstOrDefaultAsync(cancellationToken);

            company.Settings = await db.CompanySettings.AsNoTrackingWithIdentityResolution()
                                       .Where(q => q.CompanyId == companyId && access.ViewSettingsAllowedId.Contains(q.ProviderCompanyId))
                                       .Include(q => q.ProviderCompany).ToListAsync(cancellationToken);
            company.ClientSettings = await db.CompanySettings.AsNoTrackingWithIdentityResolution()
                                             .Where(q => q.ProviderCompanyId == companyId && access.ViewClientSettingsAllowedId.Contains(q.CompanyId))
                                             .Include(q => q.Company).ToListAsync(cancellationToken);
            return company;
        }

        #endregion
    }
}