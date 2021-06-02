using WebLicense.Core.Models.Identity;

namespace WebLicense.Shared.Customers
{
    public sealed record CustomerUserInfo
    {
        public long? Id { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }

        #region C-tor

        public CustomerUserInfo()
        {
        }

        public CustomerUserInfo(User user)
        {
            if (user == null) return;

            Id = user.Id;
            Name = user.UserName;
            Email = user.Email;
        }

        #endregion
    }
}