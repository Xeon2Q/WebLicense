using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Core.Models.Identity
{
    public sealed class User : IdentityUser<long>
    {
        public bool EulaAccepted { get; set; }

        public bool GdprAccepted { get; set; }

        // navigation
        public Customer Customer { get; set; }

        public ICollection<Customer> AdministeredCustomers { get; set; }
        public ICollection<CustomerAdministrator> CustomerAdministrators { get; set; }

        public ICollection<Customer> ManagedCustomers { get; set; }
        public ICollection<CustomerManager> CustomerManagers { get; set; }
    }
}