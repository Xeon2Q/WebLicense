using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebLicense.Core.Models.Companies;
using WebLicense.Shared.Customers;

namespace WebLicense.Shared.Companies
{
    public sealed class CompanyInfo
    {
        public int? Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(40)]
        public string ReferenceId { get; set; }

        // navigation
        public ICollection<CompanyUserInfo> Users { get; set; }

        public ICollection<CustomerInfo> Customers { get; set; }

        #region C-tor

        public CompanyInfo()
        {
        }

        public CompanyInfo(Company company)
        {
            if (company == null) return;

            Id = company.Id;
            Name = company.Name;
            ReferenceId = company.ReferenceId;

            if (company.Users != null && company.Users.Any()) Users = company.Users.Select(q => new CompanyUserInfo(q)).ToList();
            if (company.Customers != null && company.Customers.Any()) Customers = company.Customers.Select(q => new CustomerInfo(q, company)).ToList();
        }

        #endregion
    }
}