﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebLicense.Core.Models.Companies;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Companies
{
    public sealed record CompanyInfo
    {
        public int? Id { get; set; }

        [Required, Display(ResourceType = typeof(Model), Name = "Customer_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Code")]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_ReferenceId")]
        public string ReferenceId { get; set; }

        public CompanySettingsInfo Settings { get; set; }

        public ICollection<CompanyUserInfo> Users { get; set; }

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

            if (company.Settings != null && company.Settings.Any()) Settings = new CompanySettingsInfo(company.Settings.FirstOrDefault(q => q.ServiceConsumerCompanyId == company.Id));
            if (company.Users != null && company.Users.Any()) Users = company.Users.Select(q => new CompanyUserInfo(q)).ToList();
        }

        #endregion
    }
}