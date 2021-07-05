using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebLicense.Core.Models.Companies;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Core.Models.Identity
{
    public sealed class User : IdentityUser<long>
    {
        public bool EulaAccepted { get; set; }

        public bool GdprAccepted { get; set; }

        // navigation
        public ICollection<Company> Companies { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }

        public ICollection<Customer> ManagedCustomers { get; set; }
        public ICollection<CustomerManager> CustomerManagers { get; set; }

        public ICollection<Customer> MemberOfCustomers { get; set; }
        public ICollection<CustomerUser> CustomerUsers { get; set; }
    }
}