using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Resources;
using WebLicense.Access;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class GetCompany : IRequest<CaseResult<CompanyInfo>>, IValidate
    {
        internal int Id { get; }

        public GetCompany(int id)
        {
            Id = id;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
        }
    }

    internal sealed class GetCompanyHandler : IRequestHandler<GetCompany, CaseResult<CompanyInfo>>
    {
        private readonly DatabaseContext db;

        public GetCompanyHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult<CompanyInfo>> Handle(GetCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var company = await db.Companies.AsNoTrackingWithIdentityResolution().Where(q => q.Id == request.Id)
                                       .Include(q => q.Users)
                                       .Include(q => q.Settings)
                                       .Include(q => q.ClientSettings)
                                       .FirstOrDefaultAsync(cancellationToken);

                if (company == null) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"Company({request.Id}) not found or deleted");

                return new CaseResult<CompanyInfo>(new CompanyInfo(company));
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }
    }
}