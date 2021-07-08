using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Companies;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class AddCompany : IRequest<CaseResult<CompanyInfo>>, IValidate
    {
        internal CompanyInfo Company { get; }

        public AddCompany(CompanyInfo company)
        {
            Company = company;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Company.Name)) throw new CaseException(Exceptions.Company_Name_Empty, "Company 'Name' is empty");
            if (Company.Users == null || Company.Users.Count == 0) throw new CaseException(Exceptions.Company_Users_Empty, "Company 'Users' is empty");
            if (Company.Users.All(q => q == null || !q.Id.HasValue || q.Id < 1)) throw new CaseException(Exceptions.Company_Users_Invalid, "Company 'Users' are invalid");
        }
    }

    internal sealed class AddCompanyHandler : IRequestHandler<AddCompany, CaseResult<CompanyInfo>>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public AddCompanyHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult<CompanyInfo>> Handle(AddCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var users = request.Company.Users.Where(q => q?.Id > 0).Select(q => q.Id.Value).Distinct().Select(q => new CompanyUser {UserId = q}).ToList();
                var company = new Company
                {
                    Name = request.Company.Name,
                    Code = string.Empty.GetRandom(50),
                    ReferenceId = Guid.NewGuid().ToString("N"),
                    Logo = request.Company.Logo,
                    CompanyUsers = users,
                    Settings = null
                };

                var result = await db.Companies.AddAsync(company, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCompany(result.Entity.Id), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }
    }
}