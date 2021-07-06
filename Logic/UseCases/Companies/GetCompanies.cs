using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class GetCompanies : IRequest<CaseResult<ListData<CompanyInfo>>>, IValidate
    {
        internal Criteria<Company> Criteria { get; }

        public GetCompanies(Criteria<Company> criteria)
        {
            Criteria = criteria;
        }

        public GetCompanies(int skip, int take, string sort, bool sortAsc, Expression<Func<Company, bool>> filter) : this(new Criteria<Company>(skip, take, sort, sortAsc, filter))
        {
        }

        public GetCompanies() : this(new Criteria<Company>(0, 25, nameof(Company.Name), true, null))
        {
        }

        public void Validate()
        {
            if (Criteria == null) throw new CaseException(Exceptions.Criteria_Null, "'Criteria' is null");
        }
    }

    internal sealed class GetCompaniesHandler : IRequestHandler<GetCompanies, CaseResult<ListData<CompanyInfo>>>
    {
        private readonly DatabaseContext db;

        public GetCompaniesHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<ListData<CompanyInfo>>> Handle(GetCompanies request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var total = await request.Criteria.GetTotal(db.Set<Company>().AsNoTrackingWithIdentityResolution(), cancellationToken);
                var totalFiltered = await request.Criteria.GetTotalFiltered(db.Set<Company>().AsNoTrackingWithIdentityResolution(), total, cancellationToken);
                var data = (await request.Criteria.GetData(db.Set<Company>().AsNoTrackingWithIdentityResolution(), cancellationToken)).Select(q => new CompanyInfo(q)).ToList();

                return new CaseResult<ListData<CompanyInfo>>(new ListData<CompanyInfo>(total, totalFiltered, data));
            }
            catch (Exception e)
            {
                return new CaseResult<ListData<CompanyInfo>>(e);
            }
        }
    }
}