using System.ComponentModel.DataAnnotations;
using WebLicense.Core.Models.Companies;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Companies
{
    public sealed record CompanyUserInfo
    {
        public long? Id { get; init; }

        [Display(ResourceType = typeof(Model), Name = "User_Name")]
        public string Name { get; init; }

        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Model), Name = "User_Email")]
        public string Email { get; init; }

        [Display(ResourceType = typeof(Model), Name = "CompanyUser_IsManager")]
        public bool? IsManager { get; set; }

        [Display(ResourceType = typeof(Model), Name = "CompanyUser_IsInvite")]
        public bool IsInvite { get; set; }

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
            IsInvite = false;
        }

        public CompanyUserInfo(CompanyUserInvite user)
        {
            if (user == null) return;

            Id = user.CompanyId;
            Name = null;
            Email = user.Email;
            IsManager = false;
            IsInvite = true;
        }

        #endregion
    }
}