using MediatR;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        internal long CurrentUserId { get; }

        public AddCompany(CompanyInfo company, long currentUserId)
        {
            Company = company;
            CurrentUserId = currentUserId;
        }

        public void Validate()
        {
            if (CurrentUserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'CurrentUserId' < 1");
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

                var company = new Company
                {
                    Name = request.Company.Name,
                    Code = string.Empty.GetRandom(50),
                    ReferenceId = Guid.NewGuid().ToString("N"),
                    Logo = request.Company.Logo,
                    Settings = null
                };
                company.CompanyUsers = GetUsers(company, request.Company.Users);
                company.CompanyUserInvites = GetUserInvites(company, request.Company.Users);

                var result = await db.Companies.AddAsync(company, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return await sender.Send(new GetCompany(result.Entity.Id, request.CurrentUserId), cancellationToken);
            }
            catch (Exception e)
            {
                return new CaseResult<CompanyInfo>(e);
            }
        }

        #region Methods

        private static List<CompanyUser> GetUsers(Company company, IEnumerable<CompanyUserInfo> users)
        {
            var result = users?.Where(q => q != null && !q.IsInvite && q.Id > 0).GroupBy(q => q.Id).Select(q => q.FirstOrDefault(w => w.IsManager == true) ?? q.FirstOrDefault()).ToList();

            return result == null || !result.Any()
                ? new List<CompanyUser>(0)
                : result.Select(q => new CompanyUser {Company = company, UserId = q.Id ?? -1, IsManager = q.IsManager == true}).ToList();
        }

        private static List<CompanyUserInvite> GetUserInvites(Company company, IEnumerable<CompanyUserInfo> users)
        {
            var result = users?.Where(q => q != null && q.IsInvite).GroupBy(q => q.Email).Select(q => q.FirstOrDefault(w => w.IsManager == true) ?? q.FirstOrDefault()).ToList();

            return result == null || !result.Any()
                ? new List<CompanyUserInvite>(0)
                : result.Select(q => new CompanyUserInvite {Company = company, Email = q.Email, IsManager = q.IsManager == true, Processed = false}).ToList();
        }

        #endregion
    }
}