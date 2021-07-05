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

                var query = db.Set<Company>().AsNoTrackingWithIdentityResolution();
                request.Criteria?.ApplyAll(ref query);

                var data1 = await query.ToListAsync(cancellationToken);
                var data2 = data1.Select(q => new CompanyInfo(q)).ToList();

                var total = await db.Set<Company>().CountAsync(cancellationToken);

                return new CaseResult<ListData<CompanyInfo>>(new ListData<CompanyInfo>(total, data2));
            }
            catch (Exception e)
            {
                return new CaseResult<ListData<CompanyInfo>>(e);
            }
        }
    }
}