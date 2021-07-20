using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebLicense.Core.Models.Companies;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Companies
{
    public sealed record CompanyInfo
    {
        public int? Id { get; set; }

        [Required, Display(ResourceType = typeof(Model), Name = "Company_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Company_Code")]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Company_ReferenceId")]
        public string ReferenceId { get; set; }

        public byte[] Logo { get; set; }

        public CompanySettingsInfo Settings { get; set; }

        public List<CompanyUserInfo> Users { get; set; }

        #region C-tor

        public CompanyInfo()
        {
        }

        public CompanyInfo(Company company)
        {
            if (company == null) return;

            Id = company.Id;
            Name = company.Name;
            Code = company.Code;
            ReferenceId = company.ReferenceId;
            Logo = company.Logo;

            if (company.Settings != null && company.Settings.Any()) Settings = new CompanySettingsInfo(company.Settings.FirstOrDefault(q => q.CompanyId == company.Id));

            Users = new List<CompanyUserInfo>();

            if (company.Users != null && company.Users.Any()) Users.AddRange(company.CompanyUsers.Select(q => new CompanyUserInfo(q)));
            if (company.CompanyUserInvites != null && company.CompanyUserInvites.Any()) Users.AddRange(company.CompanyUserInvites.Select(q => new CompanyUserInfo(q)));
        }

        #endregion
    }
}