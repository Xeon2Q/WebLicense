using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebLicense.Core.Models.Companies;
using WebLicense.Core.Models.Customers;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Customers
{
    public sealed record CustomerInfo
    {
        public int? Id { get; set; }

        [Required, Display(ResourceType = typeof(Model), Name = "Customer_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Code")]
        public string Code { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_ReferenceId")]
        public string ReferenceId { get; set; }

        public CustomerSettingsInfo Settings { get; set; }

        public ICollection<CustomerUserInfo> Managers { get; set; }

        public ICollection<CustomerUserInfo> Users { get; set; }

        #region C-tor

        public CustomerInfo()
        {
        }

        public CustomerInfo(Customer customer, Company company)
        {
            if (customer == null) return;

            Id = customer.Id;
            Name = customer.Name;
            Code = customer.Code;
            ReferenceId = customer.ReferenceId;

            if (customer.Settings != null && customer.Settings.Any()) Settings = new CustomerSettingsInfo(customer.Settings.FirstOrDefault(q => q.CompanyId == company?.Id));
            if (customer.Managers != null && customer.Managers.Any()) Managers = customer.Managers.Select(q => new CustomerUserInfo(q)).ToList();
            if (customer.Users != null && customer.Users.Any()) Users = customer.Users.Select(q => new CustomerUserInfo(q)).ToList();
        }

        #endregion
    }
}