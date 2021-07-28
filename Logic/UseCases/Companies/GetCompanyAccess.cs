using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Enums;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class GetCompanyAccess : IRequest<CompanyAccessInfo>
    {
        internal int CompanyId { get; }
        internal long UserId { get; }

        public GetCompanyAccess(int companyId, long userId)
        {
            CompanyId = companyId;
            UserId = userId;
        }
    }

    internal sealed class GetCompanyAccessHandler : IRequestHandler<GetCompanyAccess, CompanyAccessInfo>
    {
        private readonly DatabaseContext db;

        public GetCompanyAccessHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CompanyAccessInfo> Handle(GetCompanyAccess request, CancellationToken cancellationToken)
        {
            if (request.UserId < 1 || request.CompanyId < 1) return new CompanyAccessInfo();

            // is admin
            var isAdmin = await db.UserRoles.AnyAsync(q => q.UserId == request.UserId && q.RoleId == Roles.AdminId, cancellationToken);
            if (isAdmin) return new CompanyAccessInfo(true, false, false);

            // user's companies :
            var membership = await db.CompanyUsers.Where(q => q.UserId == request.UserId).Select(q => new {q.CompanyId, q.IsManager}).ToArrayAsync(cancellationToken);
            var membershipId = membership.Select(q => q.CompanyId).Distinct().ToArray();

            // is company user
            var companyUser = membership.FirstOrDefault(q => q.CompanyId == request.CompanyId);
            if (companyUser != null) return new CompanyAccessInfo(false, companyUser.IsManager, true);

            // is provider
            var providers = await db.CompanySettings.Where(q => q.CompanyId == request.CompanyId && membershipId.Contains(q.ProviderCompanyId))
                                    .Select(q => q.ProviderCompanyId)
                                    .Distinct().ToArrayAsync(cancellationToken);
            var editProviders = providers.Where(q => membership.Any(w => w.CompanyId == q && w.IsManager)).ToArray();

            // is client
            var clients = await db.CompanySettings.Where(q => q.ProviderCompanyId == request.CompanyId && membershipId.Contains(q.CompanyId))
                                  .Select(q => q.CompanyId)
                                  .Distinct().ToArrayAsync(cancellationToken);
            var editClients = clients.Where(q => membership.Any(w => w.CompanyId == q && w.IsManager)).ToArray();

            return new CompanyAccessInfo(providers.Any(), providers, editProviders, clients.Any(), clients, editClients);
        }
    }
}