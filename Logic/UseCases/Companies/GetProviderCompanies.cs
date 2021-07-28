using MediatR;
using Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class GetProviderCompanies : IRequest<CaseResult<ListData<CompanyInfo>>>, IValidate
    {
        internal Criteria<Company> Criteria { get; }
        internal long CurrentUserId { get; }

        public GetProviderCompanies(Criteria<Company> criteria, long currentUserId)
        {
            Criteria = criteria;
            CurrentUserId = currentUserId;
        }

        public GetProviderCompanies(int skip, int take, string sort, bool sortAsc, Expression<Func<Company, bool>> filter, long currentUserId) : this(new Criteria<Company>(skip, take, sort, sortAsc, filter), currentUserId)
        {
        }

        public GetProviderCompanies(long currentUserId) : this(new Criteria<Company>(0, 25, nameof(Company.Name), true, null), currentUserId)
        {
        }

        public void Validate()
        {
            if (Criteria == null) throw new CaseException(Exceptions.Criteria_Null, "'Criteria' is null");
        }
    }

    internal sealed class GetProviderCompaniesHandler : IRequestHandler<GetProviderCompanies, CaseResult<ListData<CompanyInfo>>>
    {
        private readonly DatabaseContext db;

        public GetProviderCompaniesHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<ListData<CompanyInfo>>> Handle(GetProviderCompanies request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var query = db.Set<CompanyUser>().AsNoTrackingWithIdentityResolution().AsSplitQuery()
                              .Where(q => q.UserId == request.CurrentUserId)
                              .SelectMany(q => q.Company.Settings)
                              .Select(q => q.ProviderCompany).Distinct();

                var total = await request.Criteria.GetTotal(query, cancellationToken);
                var totalFiltered = await request.Criteria.GetTotalFiltered(query, total, cancellationToken);
                var data = (await request.Criteria.GetData(query, cancellationToken)).Select(q => new CompanyInfo(q)).ToList();

                return new CaseResult<ListData<CompanyInfo>>(new ListData<CompanyInfo>(total, totalFiltered, data));
            }
            catch (Exception e)
            {
                return new CaseResult<ListData<CompanyInfo>>(e);
            }
        }
    }
}