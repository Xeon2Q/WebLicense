using WebLicense.Core.Models.Companies;

namespace WebLicense.Shared.Identity
{
    public class UserCompanyInfo
    {
        public int? Id { get; init; }

        public string Name { get; init; }

        public string Code { get; init; }

        #region C-tor

        public UserCompanyInfo()
        {
        }

        public UserCompanyInfo(Company company)
        {
            if (company == null) return;

            Id = company.Id;
            Name = company.Name;
            Code = company.Code;
        }

        #endregion
    }
}