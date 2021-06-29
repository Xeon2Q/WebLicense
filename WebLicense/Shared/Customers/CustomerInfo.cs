using System.Collections.Generic;
using System.Linq;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Shared.Customers
{
    public sealed record CustomerInfo
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ReferenceId { get; set; }

        public CustomerSettingsInfo Settings { get; set; }

        public ICollection<CustomerUserInfo> Administrators { get; set; }

        public ICollection<CustomerUserInfo> Managers { get; set; }

        public ICollection<CustomerUserInfo> Users { get; set; }

        #region C-tor

        public CustomerInfo()
        {
        }

        public CustomerInfo(Customer customer)
        {
            if (customer == null) return;

            Id = customer.Id;
            Name = customer.Name;
            Code = customer.Code;
            ReferenceId = customer.ReferenceId;

            if (customer.Settings != null) Settings = new CustomerSettingsInfo(customer.Settings);

            if (customer.Administrators != null && customer.Administrators.Any()) Administrators = customer.Administrators.Select(q => new CustomerUserInfo(q)).ToList();
            if (customer.Managers != null && customer.Managers.Any()) Managers = customer.Managers.Select(q => new CustomerUserInfo(q)).ToList();
            if (customer.Users != null && customer.Users.Any()) Users = customer.Users.Select(q => new CustomerUserInfo(q)).ToList();
        }

        #endregion
    }
}