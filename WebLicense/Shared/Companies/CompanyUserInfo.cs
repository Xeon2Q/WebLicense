using WebLicense.Core.Models.Companies;

namespace WebLicense.Shared.Companies
{
    public sealed record CompanyUserInfo
    {
        public long? Id { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }

        public bool? IsManager { get; set; }

        #region C-tor

        public CompanyUserInfo()
        {
        }

        public CompanyUserInfo(CompanyUser user)
        {
            if (user == null) return;

            Id = user.UserId;
            Name = user.User?.UserName;
            Email = user.User?.Email;
            IsManager = user.IsManager;
        }

        #endregion
    }
}