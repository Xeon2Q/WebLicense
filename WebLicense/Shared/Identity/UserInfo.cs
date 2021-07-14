using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebLicense.Core.Models.Identity;
using WebLicense.Shared.Companies;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Identity
{
    public sealed class UserInfo
    {
        public long? Id { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_EmailConfirmed")]
        public bool? EmailConfirmed { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_Name")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_LockoutEnabled")]
        public bool? LockoutEnabled { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_LockoutEnd")]
        public DateTimeOffset? LockoutEnd { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_Phone")]
        public string PhoneNumber { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_PhoneConfirmed")]
        public bool? PhoneNumberConfirmed { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_2FAEnabled")]
        public bool? TwoFactorEnabled { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_EulaAccepted")]
        public bool? EulaAccepted { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_GdprAccepted")]
        public bool? GdprAccepted { get; set; }

        [Display(ResourceType = typeof(Model), Name = "User_Companies")]
        public IList<CompanyInfo> Companies { get; set; }

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
            Companies = user.Companies?.Select(q => new CompanyInfo(q)).ToList();
        }

        #endregion
    }
}