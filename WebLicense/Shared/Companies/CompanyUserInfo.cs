using WebLicense.Core.Models.Identity;

namespace WebLicense.Shared.Companies
{
    public sealed record CompanyUserInfo
    {
        public long? Id { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }

        #region C-tor

        public CompanyUserInfo()
        {
        }

        public CompanyUserInfo(User user)
        {
            if (user == null) return;

            Id = user.Id;
            Name = user.UserName;
            Email = user.Email;
        }

        #endregion
    }
}