using System;
using System.Collections.Generic;
using System.Linq;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Shared.Identity
{
    public sealed class UserInfo
    {
        public long? Id { get; set; }

        public string Email { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string UserName { get; set; }

        public bool? LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string PhoneNumber { get; set; }

        public bool? PhoneNumberConfirmed { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        public bool? EulaAccepted { get; set; }

        public bool? GdprAccepted { get; set; }

        public IList<UserCustomerInfo> AdministeredCustomers { get; set; }

        public IList<UserCustomerInfo> ManagedCustomers { get; set; }

        public IList<UserCustomerInfo> MemberOfCustomers { get; set; }

        #region C-tor

        public UserInfo()
        {
        }

        public UserInfo(User user)
        {
            if (user == null) return;

            Id = user.Id;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            UserName = user.UserName;
            LockoutEnabled = user.LockoutEnabled;
            LockoutEnd = user.LockoutEnd;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            TwoFactorEnabled = user.TwoFactorEnabled;
            EulaAccepted = user.EulaAccepted;
            GdprAccepted = user.GdprAccepted;
            AdministeredCustomers = user.AdministeredCustomers?.Select(q => new UserCustomerInfo(q)).ToList();
            ManagedCustomers = user.ManagedCustomers?.Select(q => new UserCustomerInfo(q)).ToList();
            MemberOfCustomers = user.MemberOfCustomers?.Select(q => new UserCustomerInfo(q)).ToList();
        }

        #endregion
    }
}